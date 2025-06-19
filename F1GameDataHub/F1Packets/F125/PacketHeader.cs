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

    public enum PacketId : byte
    {
        Motion = 0,
        Session = 1,
        LapData = 2,
        Event = 3,
        Participants = 4,
        CarSetups = 5,
        CarTelemetry = 6,
        CarStatus = 7,
        FinalClassification = 8,
        LobbyInfo = 9,
        CarDamage = 10,
        SessionHistory = 11,
        TyreSets = 12,
        MotionEx = 13,
        TimeTrial = 14,
        LapPositions = 15
    }

    [InlineArray(PacketConstants.MaxParticipantNameLen)]
    public struct NameBuffer
    {
        private byte _element0;
    }

    [InlineArray(4)]
    public struct TyreDataBuffer<T> where T : unmanaged
    {
        private T _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public ushort PacketFormat;              // 2025
        public byte GameYear;                    // e.g. 25
        public byte GameMajorVersion;            // X.00
        public byte GameMinorVersion;            // 1.XX
        public byte PacketVersion;               // packet version
        public PacketId PacketId;                // packet type
        public ulong SessionUid;                 // unique session ID
        public float SessionTime;                // session timestamp (seconds)
        public uint FrameIdentifier;             // frame ID
        public uint OverallFrameIdentifier;      // overall frame ID, monotonic
        public byte PlayerCarIndex;              // player car index
        public byte SecondaryPlayerCarIndex;    // secondary player car index, 255 if none
    }
}