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
                        SpinWait.SpinUntil(() => stopwatch.Elapsed >= difference);
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
                    Console.WriteLine($"{timestamp}\t Latency: {latency.TotalMilliseconds}ms");

                    // Read and skip the UDP buffer (binary data)
                    int udpBufferLength = reader.ReadInt32(); // Read length of UDP buffer
                    var udpBuffer = reader.ReadBytes(udpBufferLength);

                    await SendPacket(udpBuffer, cancellationToken);
                }
                catch (EndOfStreamException)
                {
                    break; // End of file reached
                }
            }
        }

        private async Task SendPacket(byte[] buffer, CancellationToken cancellationToken)
        {
            await udpClient.SendAsync(buffer, cancellationToken);
        }
    }
}