using F1.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class PacketConstants
    {
        public const int MaxNumCarsInUdpData = 22;
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

        /// <summary>
        /// The format of the packet, e.g. <c>2025</c> for F1 2025.
        /// </summary>
        public readonly ushort PacketFormat => _packetFormat;

        /// <summary>
        /// The last two digits of the game year, e.g. <c>25</c>.
        /// </summary>
        public readonly byte GameYear => _gameYear;

        /// <summary>
        /// The major part of the game version.
        /// </summary>
        public readonly byte GameMajorVersion => _gameMajorVersion;

        /// <summary>
        /// The minor part of the game version.
        /// </summary>
        public readonly byte GameMinorVersion => _gameMinorVersion;

        /// <summary>
        /// The version of this packet type. The first version of a given packet type is 1.
        /// </summary>
        public readonly byte PacketVersion => _packetVersion;

        /// <summary>
        /// The identifier for the packet type. See <see cref="F1PacketId"/> for possible values.
        /// </summary>
        public readonly F1PacketId PacketId => _packetId;

        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public readonly ulong SessionUid => _sessionUid;

        /// <summary>
        /// The current session time in seconds.
        /// </summary>
        public readonly float SessionTime => _sessionTime;

        /// <summary>
        /// The frame identifier for the frame the data was retrieved on.
        /// </summary>
        public readonly uint FrameIdentifier => _frameIdentifier;

        /// <summary>
        /// The overall identifier for the frame the data was retreived on.
        /// Does not go back after a flashback.
        /// </summary>
        public readonly uint OverallFrameIdentifier => _overallFrameIdentifier;

        /// <summary>
        /// The index of the player car in the session.
        /// </summary>
        public readonly byte PlayerCarIndex => _playerCarIndex;

        /// <summary>
        /// The index of the secondary player car in the session, if applicable. Otherwise, <c>null</c>.
        /// </summary>
        public readonly byte? SecondaryPlayerCarIndex =>
            _secondaryPlayerCarIndex == 255 ? null : _secondaryPlayerCarIndex;
    }
}