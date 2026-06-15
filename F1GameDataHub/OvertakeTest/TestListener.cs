using F1.Common;
using F1.Common.Lookups;
using F1Packets;
using F1Packets.F125;
using F126 = F1Packets.F126;
using Npgsql;
using PacketRecording;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace OvertakeTest
{
    /// <summary>
    /// Used for recording incoming raw UDP data for the purpose of later replaying it.
    /// </summary>
    public class TestListener(IPacketReceiver packetReceiver)
    {
        private readonly Channel<TelemetrySample> _telemetryChannel = Channel.CreateUnbounded<TelemetrySample>();

        private readonly Guid _appContextId = Guid.NewGuid(); // Unique identifier for the application context

        private readonly record struct TelemetrySample(
            long FrameId,
            int DriverIndex,
            long SessionUid,
            float Throttle,
            float Brake,
            float Steering,
            int Speed);

        /// <summary>
        /// Starts listening to the UDP port and records incoming packets to a file.
        /// Attaches a timestamp to each packet to allow for later replaying to have the same timing as when they were recorded.
        /// </summary>
        /// <param name="filePath">The path where the recorded packet file will be saved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests, allowing the recording to be stopped gracefully.</param>
        /// <returns>A task representing the asynchronous recording operation.</returns>
        public async Task StartRecording(CancellationToken cancellationToken)
        {
            var connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            var teamService = new LookupService<LookupItem>("team-ids.json");
            var nationService = new LookupService<LookupItem>("nation-ids.json");
            var driverService = new LookupService<LookupItem>("driver-carIndex.json");

            var currentLap = Enumerable.Repeat(1, 22).ToArray();
            var currentStatus = Enumerable.Repeat(ResultStatus.Invalid, 22).ToArray();
            var captureLapTime = Enumerable.Repeat(false, 22).ToArray();

            var capture = true;

            using var writer = new StreamWriter(@"D:\Temp\test.csv");
            writer.WriteLine("Car,Lap,Last Lap Time (ms)");

            int[] collisions = Enumerable.Repeat(0, 22).ToArray();

            // Loop to receive packets until cancellation is requested
            while (!cancellationToken.IsCancellationRequested)
            {
                // Recieve a UDP packet
                var udpResult = await packetReceiver.ReceiveAsync(cancellationToken);
                if (!PacketInspector.TryReadHeader(udpResult.Buffer, out var header))
                {
                    Debug.WriteLine($"Packet too short. Length: {udpResult.Buffer.Length}");
                    continue;
                }

                switch ((F1PacketId)header.PacketId)
                {
                    case F1PacketId.Session:
                        var sessionPacket = Utils.ReadFromBytes<PacketSessionData>(udpResult.Buffer);
                        await WriteSessionMetadata(conn, sessionPacket);
                        await WriteSessionState(conn, sessionPacket);
                        break;
                    case F1PacketId.Participants:
                        if (header.PacketFormat == PacketFormats.F126)
                        {
                            var participantsPacket = Utils.ReadFromBytes<F126.PacketParticipantsData>(udpResult.Buffer);
                            await WriteParticipants(conn, participantsPacket);
                        }
                        else if (header.PacketFormat == PacketFormats.F125)
                        {
                            var participantsPacket = Utils.ReadFromBytes<PacketParticipantsData>(udpResult.Buffer);
                            await WriteParticipants(conn, participantsPacket);
                        }
                        else
                        {
                            Debug.WriteLine($"Unsupported packet format for participants packet: {header.PacketFormat}");
                        }

                        break;
                    case F1PacketId.CarTelemetry:
                        if (header.PacketFormat == PacketFormats.F126)
                        {
                            var telemetryPacket = Utils.ReadFromBytes<F126.PacketCarTelemetryData>(udpResult.Buffer);
                            await WriteTelemetrySamples(telemetryPacket, cancellationToken);
                        }
                        else if (header.PacketFormat == PacketFormats.F125)
                        {
                            var telemetryPacket = Utils.ReadFromBytes<PacketCarTelemetryData>(udpResult.Buffer);
                            await WriteTelemetrySamples(telemetryPacket, cancellationToken);
                        }
                        else
                        {
                            Debug.WriteLine($"Unsupported packet format for telemetry packet: {header.PacketFormat}");
                        }

                        break;
                    case F1PacketId.CarTelemetry2:
                        if (header.PacketFormat == PacketFormats.F126)
                        {
                            _ = Utils.ReadFromBytes<F126.PacketCarTelemetry2Data>(udpResult.Buffer);
                        }

                        break;
                    default:
                        Debug.WriteLine($"Unknown sessionPacket type: {header.PacketId}");
                        break;
                }
            }
            
            Console.WriteLine(_telemetryChannel.Reader.Count);
        }

        private async Task WriteSessionState(NpgsqlConnection conn, PacketSessionData sessionPacket)
        {
            var sampleCount = sessionPacket.NumWeatherForecastSamples;
            var sb = new StringBuilder();
            Console.WriteLine($"{sessionPacket.Header.FrameIdentifier},{sessionPacket.WeatherForecastSamples[0].RainPercentage}");
            //for (int i = 0; i < sampleCount; i++)
            //{
            //    sb.AppendLine($"{sessionPacket.WeatherForecastSamples[i].TimeOffset}: {sessionPacket.WeatherForecastSamples[i].RainPercentage} | {sessionPacket.WeatherForecastSamples[i].Weather.ToString()}");
            //}
            //Console.WriteLine(sb.ToString());
        }

        public async Task HandleTelemetry(CancellationToken token)
        {
            var buffer = new List<TelemetrySample>();
            var flushInterval = TimeSpan.FromMilliseconds(100);
            //var timer = new PeriodicTimer(flushInterval);

            while (await _telemetryChannel.Reader.WaitToReadAsync(token))
            {
                // Drain as many items as have arrived since the last loop‑turn
                while (_telemetryChannel.Reader.TryRead(out var sample) && buffer.Count < 3000)
                {
                    buffer.Add(sample);
                }

                // If the interval elapsed (or cancellation requested), flush
                if (buffer.Count > 0)// && (buffer.Count > 100 || await timer.WaitForNextTickAsync(token)))
                {
                    //Console.WriteLine(buffer.Count);
                    await WriteBatchToDbAsync(buffer, token);
                    buffer.Clear();
                }
            }
        }

        async Task WriteBatchToDbAsync(List<TelemetrySample> batch, CancellationToken token)
        {
            if (batch.Count == 0) return;

            var connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync(token);

            var sb = new StringBuilder();
            sb.Append("INSERT INTO car_telemetry (frame_id, driver_index, session_uid, throttle, brake, steering, speed) VALUES ");

            var parameters = new List<NpgsqlParameter>();
            int paramIndex = 0;

            for (int idx = 0; idx < batch.Count; idx++)
            {
                var sample = batch[idx];
                var pFrameId = $"@frameId{paramIndex}";
                var pDriverId = $"@driverId{paramIndex}";
                var pSessionId = $"@sessionId{paramIndex}";
                var pThrottle = $"@throttle{paramIndex}";
                var pBrake = $"@brake{paramIndex}";
                var pSteering = $"@steering{paramIndex}";
                var pSpeed = $"@speed{paramIndex}";

                sb.Append($"({pFrameId}, {pDriverId}, {pSessionId}, {pThrottle}, {pBrake}, {pSteering}, {pSpeed})");

                if (idx < batch.Count - 1)
                {
                    sb.Append(", ");
                }

                parameters.Add(new NpgsqlParameter(pFrameId, sample.FrameId));
                parameters.Add(new NpgsqlParameter(pDriverId, sample.DriverIndex));
                parameters.Add(new NpgsqlParameter(pSessionId, sample.SessionUid));
                parameters.Add(new NpgsqlParameter(pThrottle, sample.Throttle));
                parameters.Add(new NpgsqlParameter(pBrake, sample.Brake));
                parameters.Add(new NpgsqlParameter(pSteering, sample.Steering));
                parameters.Add(new NpgsqlParameter(pSpeed, sample.Speed));
                paramIndex++;
            }

            sb.Append(" ON CONFLICT DO NOTHING;");

            await using var cmd = new NpgsqlCommand(sb.ToString(), conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            var x = await cmd.ExecuteNonQueryAsync(token);
        }

        private async Task WriteSessionMetadata(NpgsqlConnection conn, PacketSessionData packet)
        {
            var sessionUid = unchecked((long)packet.Header.SessionUid);

            await using var tx = await conn.BeginTransactionAsync();

            var selectCmd = new NpgsqlCommand("SELECT app_context_id FROM session_metadata WHERE session_uid = @session_uid", conn, tx);
            selectCmd.Parameters.AddWithValue("session_uid", sessionUid);

            var existingAppContextId = (Guid?)await selectCmd.ExecuteScalarAsync();

            if (existingAppContextId == null)
            {
                await InsertSessionMetadataAsync(conn, tx, packet, sessionUid);
            }
            else if (existingAppContextId != _appContextId)
            {
                var deleteCmd = new NpgsqlCommand("DELETE FROM session_metadata WHERE session_uid = @session_uid", conn, tx);
                deleteCmd.Parameters.AddWithValue("session_uid", sessionUid);
                await deleteCmd.ExecuteNonQueryAsync();

                await InsertSessionMetadataAsync(conn, tx, packet, sessionUid);
            }

            await tx.CommitAsync();
        }

        private async Task InsertSessionMetadataAsync(NpgsqlConnection conn, NpgsqlTransaction tx, PacketSessionData sessionPacket, long sessionUid)
        {
            var insertCmd = new NpgsqlCommand(@"
                INSERT INTO session_metadata (session_uid, app_context_id, player_car_index, game_year, packet_format, track_id, session_type, session_length, formula, session_start_time)
                VALUES (@session_uid, @app_context_id, @player_car_index, @game_year, @packet_format, @track_id, @session_type, @session_length, @formula, @session_start_time);
            ", conn, tx);

            insertCmd.Parameters.AddWithValue("session_uid", sessionUid);
            insertCmd.Parameters.AddWithValue("app_context_id", _appContextId);
            insertCmd.Parameters.AddWithValue("player_car_index", (int)sessionPacket.Header.PlayerCarIndex);
            insertCmd.Parameters.AddWithValue("game_year", (int)sessionPacket.Header.GameYear);
            insertCmd.Parameters.AddWithValue("packet_format", (int)sessionPacket.Header.PacketFormat);
            insertCmd.Parameters.AddWithValue("track_id", (int)sessionPacket.TrackId);
            insertCmd.Parameters.AddWithValue("session_type", sessionPacket.SessionType.ToString());
            insertCmd.Parameters.AddWithValue("session_length", sessionPacket.SessionLength.ToString());
            insertCmd.Parameters.AddWithValue("formula", sessionPacket.Formula.ToString());
            insertCmd.Parameters.AddWithValue("session_start_time", DateTime.UtcNow);

            await insertCmd.ExecuteNonQueryAsync();
        }

        private static async Task WriteParticipants(NpgsqlConnection conn, PacketParticipantsData packet)
        {
            for (int i = 0; i < PacketConstants.MaxNumCarsInUdpData; i++)
            {
                using var cmd = new NpgsqlCommand(@"
                INSERT INTO participants (
                    session_uid,
                    driver_id,
                    network_id,
                    name,
                    team_id,
                    nationality_id,
                    is_ai_controlled,
                    race_number
                )
                VALUES (
                    @session_uid,
                    @driver_id,
                    @network_id,
                    @name,
                    @team_id,
                    @nationality_id,
                    @is_ai_controlled,
                    @race_number
                )
                ON CONFLICT (session_uid, driver_id) DO NOTHING;
            ", conn);

                var sessionUid = unchecked((long)packet.Header.SessionUid);
                var name = packet.Participants[i].Name;
                if (name == null || name == "Player") name = $"Driver {i}";

                cmd.Parameters.AddWithValue("session_uid", sessionUid);
                cmd.Parameters.AddWithValue("driver_id", i);
                cmd.Parameters.AddWithValue("network_id", packet.Participants[i].NetworkId);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("team_id", packet.Participants[i].TeamId);
                cmd.Parameters.AddWithValue("nationality_id", packet.Participants[i].Nationality);
                cmd.Parameters.AddWithValue("is_ai_controlled", packet.Participants[i].AiControlled);
                cmd.Parameters.AddWithValue("race_number", packet.Participants[i].RaceNumber);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static async Task WriteParticipants(NpgsqlConnection conn, F126.PacketParticipantsData packet)
        {
            for (int i = 0; i < F126.PacketConstants.MaxNumCarsInUdpData; i++)
            {
                using var cmd = new NpgsqlCommand(@"
                INSERT INTO participants (
                    session_uid,
                    driver_id,
                    network_id,
                    name,
                    team_id,
                    nationality_id,
                    is_ai_controlled,
                    race_number
                )
                VALUES (
                    @session_uid,
                    @driver_id,
                    @network_id,
                    @name,
                    @team_id,
                    @nationality_id,
                    @is_ai_controlled,
                    @race_number
                )
                ON CONFLICT (session_uid, driver_id) DO NOTHING;
            ", conn);

                var sessionUid = unchecked((long)packet.Header.SessionUid);
                var name = packet.Participants[i].Name;
                if (name == null || name == "Player") name = $"Driver {i}";

                cmd.Parameters.AddWithValue("session_uid", sessionUid);
                cmd.Parameters.AddWithValue("driver_id", i);
                cmd.Parameters.AddWithValue("network_id", (int)packet.Participants[i].NetworkId);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("team_id", (int)packet.Participants[i].TeamId);
                cmd.Parameters.AddWithValue("nationality_id", packet.Participants[i].Nationality);
                cmd.Parameters.AddWithValue("is_ai_controlled", packet.Participants[i].AiControlled);
                cmd.Parameters.AddWithValue("race_number", packet.Participants[i].RaceNumber);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static async Task<int> WriteTelemetry(NpgsqlConnection conn, PacketCarTelemetryData packet, int carIndex)
        {
            var cmd = new NpgsqlCommand(@"
                INSERT INTO car_telemetry (
                    session_uid,
                    frame_id,
                    driver_index,
                    throttle,
                    brake,
                    steering,
                    speed
                )
                VALUES (
                    @session_uid,
                    @frame_id,
                    @driver_index,
                    @throttle,
                    @brake,
                    @steering,
                    @speed
                );
            ", conn);

            var sessionUid = unchecked((long)packet.Header.SessionUid);

            cmd.Parameters.AddWithValue("session_uid", sessionUid);
            cmd.Parameters.AddWithValue("frame_id", (int)packet.Header.FrameIdentifier);
            cmd.Parameters.AddWithValue("driver_index", carIndex);
            cmd.Parameters.AddWithValue("throttle", packet.CarTelemetryData[carIndex].Throttle);
            cmd.Parameters.AddWithValue("brake", packet.CarTelemetryData[carIndex].Brake);
            cmd.Parameters.AddWithValue("steering", packet.CarTelemetryData[carIndex].Steer);
            cmd.Parameters.AddWithValue("speed", (int)packet.CarTelemetryData[carIndex].Speed);

            return await cmd.ExecuteNonQueryAsync();
        }

        private async Task WriteTelemetrySamples(PacketCarTelemetryData packet, CancellationToken cancellationToken)
        {
            var sessionUid = unchecked((long)packet.Header.SessionUid);
            var frameId = packet.Header.FrameIdentifier;

            for (int carIndex = 0; carIndex < PacketConstants.MaxNumCarsInUdpData; carIndex++)
            {
                var telemetry = packet.CarTelemetryData[carIndex];
                await _telemetryChannel.Writer.WriteAsync(
                    new TelemetrySample(
                        frameId,
                        carIndex,
                        sessionUid,
                        telemetry.Throttle,
                        telemetry.Brake,
                        telemetry.Steer,
                        telemetry.Speed),
                    cancellationToken);
            }
        }

        private async Task WriteTelemetrySamples(F126.PacketCarTelemetryData packet, CancellationToken cancellationToken)
        {
            var sessionUid = unchecked((long)packet.Header.SessionUid);
            var frameId = packet.Header.FrameIdentifier;

            for (int carIndex = 0; carIndex < F126.PacketConstants.MaxNumCarsInUdpData; carIndex++)
            {
                var telemetry = packet.CarTelemetryData[carIndex];
                await _telemetryChannel.Writer.WriteAsync(
                    new TelemetrySample(
                        frameId,
                        carIndex,
                        sessionUid,
                        telemetry.Throttle,
                        telemetry.Brake,
                        telemetry.Steer,
                        telemetry.Speed),
                    cancellationToken);
            }
        }
    }
}