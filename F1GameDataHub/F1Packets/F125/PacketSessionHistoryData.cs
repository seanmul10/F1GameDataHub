using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class SessionHistoryPacketConstants
    {
        public const int MaxNumLapsInHistory = 100;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapHistoryData
    {
        public uint LapTimeInMS;
        public ushort Sector1TimeMSPart;
        public byte Sector1TimeMinutesPart;
        public ushort Sector2TimeMSPart;
        public byte Sector2TimeMinutesPart;
        public ushort Sector3TimeMSPart;
        public byte Sector3TimeMinutesPart;
        public byte LapValidBitFlags;
    }

    [InlineArray(SessionHistoryPacketConstants.MaxNumLapsInHistory)]
    public struct LapHistoryDataBuffer
    {
        private LapHistoryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TyreStintHistoryData
    {
        public byte EndLap;
        public byte TyreActualCompound;
        public byte TyreVisualCompound;
    }

    [InlineArray(PacketConstants.MaxTyreStints)]
    public struct TyreStintHistoryDataBuffer
    {
        private TyreStintHistoryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketSessionHistoryData
    {
        public PacketHeader Header;
        public byte CarIdx;
        public byte NumLaps;
        public byte NumTyreStints;
        public byte BestLapTimeLapNum;
        public byte BestSector1LapNum;
        public byte BestSector2LapNum;
        public byte BestSector3LapNum;
        public LapHistoryDataBuffer LapHistoryData;
        public TyreStintHistoryDataBuffer TyreStintsHistoryData;
    }
}