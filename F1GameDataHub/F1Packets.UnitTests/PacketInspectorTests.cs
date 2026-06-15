using F126 = F1Packets.F126;
using System.Runtime.InteropServices;

namespace F1Packets.UnitTests
{
    [TestClass]
    public sealed class PacketInspectorTests
    {
        [TestMethod]
        public void TryReadHeader_ReturnsExpectedValues_ForValidPacketPrefix()
        {
            var bytes = new byte[7];
            bytes[0] = 0xEA; // 2026 little-endian
            bytes[1] = 0x07;
            bytes[6] = (byte)F1.Common.F1PacketId.CarTelemetry;

            var parsed = PacketInspector.TryReadHeader(bytes, out var header);

            Assert.IsTrue(parsed);
            Assert.AreEqual(PacketFormats.F126, header.PacketFormat);
            Assert.AreEqual((byte)F1.Common.F1PacketId.CarTelemetry, header.PacketId);
        }

        [TestMethod]
        public void TryReadHeader_ReturnsFalse_ForShortPacket()
        {
            var parsed = PacketInspector.TryReadHeader([1, 2, 3], out var header);

            Assert.IsFalse(parsed);
            Assert.AreEqual(default, header);
        }

        [TestMethod]
        public void F126PacketSizes_MatchSpec()
        {
            Assert.AreEqual(1470, Marshal.SizeOf<F126.PacketParticipantsData>());
            Assert.AreEqual(1448, Marshal.SizeOf<F126.PacketCarTelemetryData>());
            Assert.AreEqual(269, Marshal.SizeOf<F126.PacketCarTelemetry2Data>());
        }
    }
}
