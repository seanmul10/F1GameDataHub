using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FinalClassificationData
    {
        public byte Position;
        public byte NumLaps;
        public byte GridPosition;
        public byte Points;
        public byte NumPitStops;
        public byte ResultStatus;
        public byte ResultReason;
        public uint BestLapTimeInMS;
        public double TotalRaceTime;
        public byte PenaltiesTime;
        public byte NumPenalties;
        public byte NumTyreStints;
        public TyreStintsDataBuffer TyreStintsActual;
        public TyreStintsDataBuffer TyreStintsVisual;
        public TyreStintsDataBuffer TyreStintsEndLaps;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct FinalClassificationDataBuffer
    {
        private FinalClassificationData _element0;
    }

    [InlineArray(PacketConstants.MaxTyreStints)]
    public struct TyreStintsDataBuffer
    {
        private byte _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketFinalClassificationData
    {
        public PacketHeader Header;
        public byte NumCars;
        public FinalClassificationDataBuffer ClassificationData;
    }
}