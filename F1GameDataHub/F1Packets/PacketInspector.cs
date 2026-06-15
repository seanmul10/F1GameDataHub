using System.Buffers.Binary;

namespace F1Packets
{
    public static class PacketFormats
    {
        public const ushort F125 = 2025;
        public const ushort F126 = 2026;
    }

    public readonly struct PacketHeaderInfo
    {
        public PacketHeaderInfo(ushort packetFormat, byte packetId)
        {
            PacketFormat = packetFormat;
            PacketId = packetId;
        }

        public ushort PacketFormat { get; }
        public byte PacketId { get; }
    }

    public static class PacketInspector
    {
        private const int PacketHeaderPrefixLength = 7;

        public static bool TryReadHeader(byte[] bytes, out PacketHeaderInfo header)
        {
            if (bytes.Length < PacketHeaderPrefixLength)
            {
                header = default;
                return false;
            }

            var packetFormat = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(0, 2));
            var packetId = bytes[6];
            header = new PacketHeaderInfo(packetFormat, packetId);
            return true;
        }
    }
}
