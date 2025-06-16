using NSubstitute;
using PacketRecording;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PacketRecording.UnitTests
{
    [TestClass]
    public sealed class PacketRecordingTests
    {
        [TestMethod]
        public async Task PacketRecording_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\test_packet.bin");

            var cts = new CancellationTokenSource();

            // Mock the IPacketReceiver to simulate receiving UDP packets
            var mockPacketReceiver = Substitute.For<IPacketReceiver>();
            mockPacketReceiver
                .ReceiveAsync(Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    cts.Cancel();
                    return Task.FromResult(new UdpReceiveResult(bytes, new IPEndPoint(IPAddress.None, 0)));
                });

            // Mock the stream factory to create a memory stream for recording
            using var testMemoryStream = new NonClosingMemoryStream();
            var mockStreamFactory = Substitute.For<IStreamFactory>();
            mockStreamFactory.
                CreateStream(Arg.Any<string>()).
                Returns(testMemoryStream);

            var udpPacketRecorder = new UdpPacketRecorder(mockPacketReceiver, mockStreamFactory);

            var preTime = DateTime.UtcNow;
            await udpPacketRecorder.StartRecording("testFilePath", cts.Token);
            var postTime = DateTime.UtcNow;

            await mockPacketReceiver.Received(1)
                .ReceiveAsync(Arg.Any<CancellationToken>());

            mockStreamFactory.Received(1)
                .CreateStream(Arg.Any<string>());

            testMemoryStream.Position = 0; // Reset the position to read from the beginning

            Assert.IsTrue(testMemoryStream.Length > 0, "The memory stream should contain data after recording.");
            
            var binaryReader = new BinaryReader(testMemoryStream);

            var timestampLength = binaryReader.ReadInt32();
            var timestampBytes = binaryReader.ReadBytes(timestampLength);
            Assert.AreEqual(timestampLength, timestampBytes.Length);
            Assert.IsTrue(DateTime.TryParseExact(
                Encoding.UTF8.GetString(timestampBytes),
                "o",
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var timestamp));

            Assert.IsTrue(timestamp >= preTime && timestamp <= postTime, "The timestamp should be within the expected range.");

            var udpBufferLength = binaryReader.ReadInt32();
            var udpBuffer = binaryReader.ReadBytes(udpBufferLength);
            Assert.AreEqual(udpBufferLength, udpBuffer.Length);
            Assert.AreEqual(testMemoryStream.Length, testMemoryStream.Position);

            testMemoryStream.CloseActual(); // Close the stream for real
        }

        private class NonClosingMemoryStream : MemoryStream
        {
            public override void Close()
            {
                // Do not close the stream to allow further operations on it
            }

            public void CloseActual()
            {
                base.Close();
            }
        }
    }
}
