using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapData
    {
        public uint LastLapTimeInMS;
        public uint CurrentLapTimeInMS;
        public ushort Sector1TimeMSPart;
        public byte Sector1TimeMinutesPart;
        public ushort Sector2TimeMSPart;
        public byte Sector2TimeMinutesPart;
        public ushort DeltaToCarInFrontMSPart;
        public byte DeltaToCarInFrontMinutesPart;
        public ushort DeltaToRaceLeaderMSPart;
        public byte DeltaToRaceLeaderMinutesPart;
        public float LapDistance;
        public float TotalDistance;
        public float SafetyCarDelta;
        public byte CarPosition;
        public byte CurrentLapNum;
        public byte PitStatus;
        public byte NumPitStops;
        public byte Sector;
        public byte CurrentLapInvalid;
        public byte Penalties;
        public byte TotalWarnings;
        public byte CornerCuttingWarnings;
        public byte NumUnservedDriveThroughPens;
        public byte NumUnservedStopGoPens;
        public byte GridPosition;
        public byte DriverStatus;
        public byte ResultStatus;
        public byte PitLaneTimerActive;
        public ushort PitLaneTimeInLaneInMS;
        public ushort PitStopTimerInMS;
        public byte PitStopShouldServePen;
        public float SpeedTrapFastestSpeed;
        public byte SpeedTrapFastestLap;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct LapDataBuffer
    {
        private LapData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLapData
    {
        public PacketHeader Header;
        public LapDataBuffer LapData;
        public byte TimeTrialPBCarIdx;
        public byte TimeTrialRivalCarIdx;
    }
}