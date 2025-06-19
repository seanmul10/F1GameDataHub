using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TimeTrialDataSet
    {
        public byte CarIdx;                  // uint8
        public byte TeamId;                  // uint8
        public uint LapTimeInMS;             // uint (UInt32)
        public uint Sector1TimeInMS;         // uint
        public uint Sector2TimeInMS;         // uint
        public uint Sector3TimeInMS;         // uint
        public byte TractionControl;         // uint8
        public byte GearboxAssist;           // uint8
        public byte AntiLockBrakes;          // uint8
        public byte EqualCarPerformance;     // uint8
        public byte CustomSetup;             // uint8
        public byte Valid;                   // uint8
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketTimeTrialData
    {
        public PacketHeader Header;

        public TimeTrialDataSet PlayerSessionBestDataSet;
        public TimeTrialDataSet PersonalBestDataSet;
        public TimeTrialDataSet RivalDataSet;
    }
}