using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using PacketRecording;

namespace PacketRecording.UnitTests
{
    [TestClass]
    public class UdpClientReceiverTests
    {
        [TestMethod]
        public void UdpClientReceiver_Constructor_NullClient_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UdpClientReceiver(null!));
        }

        [TestMethod]
        public void UdpClientReceiver_Constructor_ValidClient_DoesNotThrow()
        {
            using var udpClient = new UdpClient(0);
            var receiver = new UdpClientReceiver(udpClient);
            Assert.IsNotNull(receiver);
        }
    }
}
