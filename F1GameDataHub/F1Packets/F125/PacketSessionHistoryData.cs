using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class SessionHistoryPacketConstants
    {
        public const int MaxNumLapsInHistory = 100;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct LapHistoryData
    {
        private readonly uint _lapTimeInMS;
        private readonly ushort _sector1TimeMSPart;
        private readonly byte _sector1TimeMinutesPart;
        private readonly ushort _sector2TimeMSPart;
        private readonly byte _sector2TimeMinutesPart;
        private readonly ushort _sector3TimeMSPart;
        private readonly byte _sector3TimeMinutesPart;
        private readonly byte _lapValidBitFlags;

        /// <summary>
        /// The total lap time in milliseconds.
        /// </summary>
        public uint LapTimeInMS => _lapTimeInMS;

        /// <summary>
        /// The sector 1 time in milliseconds.
        /// </summary>
        public uint Sector1TimeInMS => (uint)(_sector1TimeMSPart + (_sector1TimeMinutesPart * 60000));

        /// <summary>
        /// The sector 2 time in milliseconds.
        /// </summary>
        public uint Sector2TimeInMS => (uint)(_sector2TimeMSPart + (_sector2TimeMinutesPart * 60000));

        /// <summary>
        /// The sector 3 time in milliseconds.
        /// </summary>
        public uint Sector3TimeInMS => (uint)(_sector3TimeMSPart + (_sector3TimeMinutesPart * 60000));

        /// <summary>
        /// The flags indicating the validity of the lap. 0x01 = lap valid, 0x02 = sector 1 valid, 0x04 = sector 2 valid, 0x08 = sector 3 valid.
        /// </summary>
        public byte LapValidBitFlags => _lapValidBitFlags;
    }

    [InlineArray(SessionHistoryPacketConstants.MaxNumLapsInHistory)]
    public struct LapHistoryDataBuffer
    {
        private LapHistoryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct TyreStintHistoryData
    {
        public readonly byte _endLap;
        public readonly byte _tyreActualCompound;
        public readonly byte _tyreVisualCompound;

        /// <summary>
        /// The lap this tyre stint ended, <c>null</c> for current stint.
        /// </summary>
        public byte? EndLap => _endLap == 255 ? null : _endLap;

        /// <summary>
        /// The actual tyre compound used during this stint.
        /// </summary>
        public byte TyreActualCompound => _tyreActualCompound;

        /// <summary>
        /// The visual tyre compound used during this stint.
        /// </summary>
        public byte TyreVisualCompound => _tyreVisualCompound;
    }

    [InlineArray(PacketConstants.MaxTyreStints)]
    public struct TyreStintHistoryDataBuffer
    {
        private TyreStintHistoryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketSessionHistoryData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _carIdx;
        private readonly byte _numLaps;
        private readonly byte _numTyreStints;
        private readonly byte _bestLapTimeLapNum;
        private readonly byte _bestSector1LapNum;
        private readonly byte _bestSector2LapNum;
        private readonly byte _bestSector3LapNum;
        private readonly LapHistoryDataBuffer _lapHistoryData;
        private readonly TyreStintHistoryDataBuffer _tyreStintsHistoryData;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The index of the car this session history data relates to.
        /// </summary>
        public byte CarIdx => _carIdx;

        /// <summary>
        /// The number of laps in the data, including the current partial lap.
        /// </summary>
        public byte NumLaps => _numLaps;

        /// <summary>
        /// The number of tyre stints in the data, including the current stint.
        /// </summary>
        public byte NumTyreStints => _numTyreStints;

        /// <summary>
        /// The lap number of the best lap time recorded in the session.
        /// </summary>
        public byte BestLapTimeLapNum => _bestLapTimeLapNum;

        /// <summary>
        /// The lap number the best sector 1 time was recorded in the session.
        /// </summary>
        public byte BestSector1LapNum => _bestSector1LapNum;

        /// <summary>
        /// The lap number the best sector 2 time was recorded in the session.
        /// </summary>
        public byte BestSector2LapNum => _bestSector2LapNum;

        /// <summary>
        /// The lap number the best sector 3 time was recorded in the session.
        /// </summary>
        public byte BestSector3LapNum => _bestSector3LapNum;

        /// <summary>
        /// The lap history data for the car in the session, containing lap times and sector times.
        /// </summary>
        public LapHistoryDataBuffer LapHistoryData => _lapHistoryData;

        /// <summary>
        /// The tyre stints history data for the car in the session, containing tyre compounds and stint lengths.
        /// </summary>
        public TyreStintHistoryDataBuffer TyreStintsHistoryData => _tyreStintsHistoryData;
    }
}