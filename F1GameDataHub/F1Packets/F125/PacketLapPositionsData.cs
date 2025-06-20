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
    public readonly struct PacketLapPositionsData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _numLaps;
        private readonly byte _lapStart;
        private readonly PositionsInLapBuffer _positionForVehicleIdx;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The number of laps in the data, up to a maximum of <see cref="LapPositionsPacketConstants.MaxNumLaps"/>.
        /// </summary>
        public byte NumLaps => _numLaps;

        /// <summary>
        /// The index of the lap where the data starts.
        /// </summary>
        public byte LapStart => _lapStart;

        /// <summary>
        /// A 2-dimensional array representing the positions of each vehicle in each lap.
        /// </summary>
        public PositionsInLapBuffer PositionForVehicleIdx => _positionForVehicleIdx;
    }
}