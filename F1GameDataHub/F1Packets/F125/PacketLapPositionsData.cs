using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class LapPositionsPacketConstants
    {
        public const int MaxNumLaps = 50;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct PositionForVehicleIdx
    {
        private byte _element0;
    }

    [InlineArray(LapPositionsPacketConstants.MaxNumLaps)]
    public struct PositionsInLapBuffer
    {
        private PositionForVehicleIdx _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLapPositionsData
    {
        public PacketHeader Header;

        public byte NumLaps;
        public byte LapStart;

        // Fixed size 2D array [50][22]
        public PositionsInLapBuffer PositionForVehicleIdx;
    }
}