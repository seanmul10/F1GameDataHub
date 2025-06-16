using System.Text;

namespace PacketRecording
{
    /// <summary>
    /// Used for recording incoming raw UDP data for the purpose of later replaying it.
    /// </summary>
    public class UdpPacketRecorder(IPacketReceiver packetReceiver, IStreamFactory streamFactory)
    {
        /// <summary>
        /// Starts listening to the UDP port and records incoming packets to a file.
        /// Attaches a timestamp to each packet to allow for later replaying to have the same timing as when they were recorded.
        /// </summary>
        /// <param name="filePath">The path where the recorded packet file will be saved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests, allowing the recording to be stopped gracefully.</param>
        /// <returns>A task representing the asynchronous recording operation.</returns>
        public async Task StartRecording(string filePath, CancellationToken cancellationToken)
        {
            using var stream = streamFactory.CreateStream(filePath);
            using var writer = new BinaryWriter(stream);

            // Loop to receive packets until cancellation is requested
            while (!cancellationToken.IsCancellationRequested)
            {
                // Recieve a UDP packet
                var udpResult = await packetReceiver.ReceiveAsync(cancellationToken);

                // Timestamp the packet
                string timeStamp = DateTime.UtcNow.ToString("o");  // ISO 8601 format for timestamp

                // Log the packet to the file
                Write(writer, udpResult.Buffer, timeStamp);
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