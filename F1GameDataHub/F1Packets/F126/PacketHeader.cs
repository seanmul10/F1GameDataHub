using F1.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F126
{
    public static class PacketConstants
    {
        public const int MaxNumCarsInUdpData = 24;
        public const int MaxParticipantNameLen = 32;
        public const int MaxTyreStints = 8;
        public const int MaxNumTyreSets = 13 + 7; // 13 slick + 7 wet weather
    }

    [InlineArray(PacketConstants.MaxParticipantNameLen)]
    public struct NameBuffer
    {
        private byte _element0;
    }

    [InlineArray(4)]
    public struct WheelDataBuffer<T> where T : unmanaged
    {
        private T _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketHeader
    {
        private readonly ushort _packetFormat;
        private readonly byte _gameYear;
        private readonly byte _gameMajorVersion;
        private readonly byte _gameMinorVersion;
        private readonly byte _packetVersion;
        private readonly F1PacketId _packetId;
        private readonly ulong _sessionUid;
        private readonly float _sessionTime;
        private readonly uint _frameIdentifier;
        private readonly uint _overallFrameIdentifier;
        private readonly byte _playerCarIndex;
        private readonly byte _secondaryPlayerCarIndex;

        public ushort PacketFormat => _packetFormat;
        public byte GameYear => _gameYear;
        public byte GameMajorVersion => _gameMajorVersion;
        public byte GameMinorVersion => _gameMinorVersion;
        public byte PacketVersion => _packetVersion;
        public F1PacketId PacketId => _packetId;
        public ulong SessionUid => _sessionUid;
        public float SessionTime => _sessionTime;
        public uint FrameIdentifier => _frameIdentifier;
        public uint OverallFrameIdentifier => _overallFrameIdentifier;
        public byte PlayerCarIndex => _playerCarIndex;
        public byte? SecondaryPlayerCarIndex =>
            _secondaryPlayerCarIndex == 255 ? null : _secondaryPlayerCarIndex;
    }
}
