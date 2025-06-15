using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PacketRecording
{
    /// <summary>
    /// Used for recording incoming raw UDP data for the purpose of later replaying it.
    /// </summary>
    /// <param name="ipEndPoint">The IP endpoint to receive the UDP packets from.</param>
    /// <param name="filePath">The path to write the recording file into.</param>
    public class UdpPacketRecorder(IPEndPoint ipEndPoint, string filePath)
    {
        private readonly IPEndPoint _ipEndPoint = ipEndPoint;
        private readonly string _directory = filePath;

        /// <summary>
        /// Starts listening to the UDP port and records incoming packets to a file.
        /// Attaches a timestamp to each packet to allow for later replaying to have the same timing as when they were recorded.
        /// </summary>
        /// <param name="compressionEnabled">If true, the recorded packets will be compressed using GZip; otherwise, they will be saved as raw binary.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests, allowing the recording to be stopped gracefully.</param>
        /// <returns>A task representing the asynchronous recording operation.</returns>
        public async Task StartRecording(bool compressionEnabled, CancellationToken cancellationToken)
        {
            using var udpClient = new UdpClient(_ipEndPoint);
            Console.WriteLine($"Listening for UDP packets on port {_ipEndPoint}...");

            var extension = compressionEnabled ? ".gz" : ".bin";
            var path = Path.Combine(_directory, extension);

            var fileStream = new FileStream(path, FileMode.Create);
            Stream writerOutputStream = fileStream;
            GZipStream? gZipStream = null;
            if (compressionEnabled)
            {
                gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
                writerOutputStream = gZipStream;
            }
            var writer = new BinaryWriter(writerOutputStream);

            try
            {
                // Loop to receive packets until cancellation is requested
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Recieve a UDP packet
                    var udpResult = await udpClient.ReceiveAsync(cancellationToken);

                    // Timestamp the packet
                    string timeStamp = DateTime.UtcNow.ToString("o");  // ISO 8601 format for timestamp

                    // Log the packet to the file
                    Write(writer, udpResult.Buffer, timeStamp);
                }
            }
            finally
            {
                // Dispose after usage:
                writer.Dispose();
                gZipStream?.Dispose();
                fileStream.Dispose();
            }
        }

        private static void Write(BinaryWriter writer, byte[] buffer, string timeStamp)
        {
            Console.WriteLine($"{timeStamp} <udpBuffer>");

            var timeStampBytes = Encoding.UTF8.GetBytes(timeStamp);
            writer.Write(timeStampBytes.Length);
            writer.Write(timeStampBytes);

            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
    }
}