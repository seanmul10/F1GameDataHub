using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace PacketRecording
{
    public class UdpClientReceiver : IPacketReceiver, IDisposable
    {
        private readonly UdpClient _client;

        public UdpClientReceiver(UdpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            var port = (_client.Client.LocalEndPoint as IPEndPoint)?.Port;
            Console.WriteLine($"Listening for UDP packets on port {port}...");
        }

        public Task<UdpReceiveResult> ReceiveAsync(CancellationToken token) =>
            _client.ReceiveAsync(token).AsTask();

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
