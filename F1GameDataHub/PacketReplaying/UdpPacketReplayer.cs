using F1.Common;
using F1Packets;
using F1Packets.F125;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PacketReplaying
{
    public class UdpPacketReplayer
    {
        private readonly string filePath;
        private readonly UdpClient udpClient;

        public UdpPacketReplayer(IPAddress ipAddress, int port, string filePath)
        {
            this.filePath = filePath;

            udpClient = new UdpClient();
            udpClient.Connect(ipAddress, port);
        }

        public async Task Replay(CancellationToken cancellationToken)
        {
            // Read the compressed file
            using var fileStream = new FileStream(filePath, FileMode.Open);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = new BinaryReader(gzipStream);

            DateTime? firstTimestamp = null; // For time difference calculation
            var stopwatch = new Stopwatch();
            var previousLatency = 0d;

            var sfp = new MutablePacketFinalClassificationData();
            var sentActualFinalClassification = false;

            // Read until the end of the file
            while (gzipStream.CanRead && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Read timestamp length and the actual timestamp (as a UTF-8 string)
                    int timestampLength = reader.ReadInt32();
                    byte[] timestampBytes = reader.ReadBytes(timestampLength);
                    string timestampStr = Encoding.UTF8.GetString(timestampBytes);

                    // Convert the timestamp string back to DateTime
                    DateTime timestamp = DateTime.Parse(timestampStr);

                    if (firstTimestamp != null)
                    {
                        var difference = timestamp.Subtract(firstTimestamp.Value);
                        //SpinWait.SpinUntil(() => stopwatch.Elapsed >= difference);
                    }
                    else
                    {
                        firstTimestamp = timestamp;
                        stopwatch.Start();
                    }

                    var packetOffset = (timestamp - firstTimestamp).Value;
                    var elapsedTime = stopwatch.Elapsed;
                    var latency = packetOffset - elapsedTime;
                    previousLatency = latency.TotalMilliseconds;

                    // Read and skip the UDP buffer (binary data)
                    int udpBufferLength = reader.ReadInt32(); // Read length of UDP buffer
                    var udpBuffer = reader.ReadBytes(udpBufferLength);

                    //switch (udpBuffer[6])
                    //{
                    //    case (byte)F1PacketId.Session:
                    //        var packet = Utils.ReadFromBytes<PacketSessionData>(udpBuffer);

                    //        // Header
                    //        sfp.Header.PacketFormat = packet.Header.PacketFormat;
                    //        sfp.Header.GameYear = packet.Header.GameYear;
                    //        sfp.Header.GameMajorVersion = packet.Header.GameMajorVersion;
                    //        sfp.Header.GameMinorVersion = packet.Header.GameMinorVersion;
                    //        sfp.Header.PacketVersion = 1;
                    //        sfp.Header.PacketId = F1PacketId.FinalClassification;
                    //        sfp.Header.SessionUid = packet.Header.SessionUid;
                    //        sfp.Header.SessionTime = packet.Header.SessionTime;
                    //        sfp.Header.FrameIdentifier = packet.Header.FrameIdentifier;
                    //        sfp.Header.OverallFrameIdentifier = packet.Header.OverallFrameIdentifier;
                    //        sfp.Header.PlayerCarIndex = packet.Header.PlayerCarIndex;
                    //        sfp.Header.SecondaryPlayerCarIndex = packet.Header.SecondaryPlayerCarIndex;
                    //        sfp.ClassificationData = new MutableFinalClassificationData[PacketConstants.MaxNumCarsInUdpData];
                    //        foreach (var car in packet.sess)
                    //        break;
                    //}

                    await SendPacket(udpBuffer, cancellationToken);

                    if ((F1PacketId)udpBuffer[6] == F1PacketId.FinalClassification)
                    {
                        sentActualFinalClassification = true;
                        for (int i = 0; i < 20; i++)
                        {
                            Console.WriteLine("Sending additional packets for Final Classification");
                            await SendPacket(udpBuffer, cancellationToken);
                        }
                    }

                    Console.WriteLine($"{timestamp}\t Latency: {latency.TotalMilliseconds}ms\t Packet: {(F1PacketId)udpBuffer[6]}");
                }
                catch (EndOfStreamException)
                {
                    if (!sentActualFinalClassification)
                    {
                        
                    }
                    break; // End of file reached
                }
            }
        }

        private async Task SendPacket(byte[] buffer, CancellationToken cancellationToken)
        {
            await udpClient.SendAsync(buffer, cancellationToken);
        }

        private class MutablePacketHeader
        {
            public ushort PacketFormat { get; set; }
            public byte GameYear { get; set; }
            public byte GameMajorVersion { get; set; }
            public byte GameMinorVersion { get; set; }
            public byte PacketVersion { get; set; }
            public F1PacketId PacketId { get; set; }
            public ulong SessionUid { get; set; }
            public float SessionTime { get; set; }
            public uint FrameIdentifier { get; set; }
            public uint OverallFrameIdentifier { get; set; }
            public byte PlayerCarIndex { get; set; }
            public byte? SecondaryPlayerCarIndex { get; set; }
        }

        private class MutableFinalClassificationData
        {
            public byte Position { get; set; }
            public byte NumLaps { get; set; }
            public byte GridPosition { get; set; }
            public byte Points { get; set; }
            public byte NumPitStops { get; set; }
            public ResultStatus ResultStatus { get; set; }
            public ResultReason ResultReason { get; set; }
            public uint BestLapTimeInMS { get; set; }
            public double TotalRaceTime { get; set; }
            public byte PenaltiesTime { get; set; }
            public byte NumPenalties { get; set; }
            public byte NumTyreStints { get; set; }
            public byte[] TyreStintsActual { get; set; } = new byte[PacketConstants.MaxTyreStints];
            public byte[] TyreStintsVisual { get; set; } = new byte[PacketConstants.MaxTyreStints];
            public byte[] TyreStintsEndLaps { get; set; } = new byte[PacketConstants.MaxTyreStints];
        }

        private class MutablePacketFinalClassificationData
        {
            public MutablePacketHeader Header { get; set; }
            public byte NumCars { get; set; }
            public MutableFinalClassificationData[] ClassificationData { get; set; }
                = new MutableFinalClassificationData[PacketConstants.MaxNumCarsInUdpData];
        }
    }
}