using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
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
    public readonly struct FinalClassificationData
    {
        private readonly byte _position;
        private readonly byte _numLaps;
        private readonly byte _gridPosition;
        private readonly byte _points;
        private readonly byte _numPitStops;
        private readonly byte _resultStatus;
        private readonly byte _resultReason;
        private readonly uint _bestLapTimeInMS;
        private readonly double _totalRaceTime;
        private readonly byte _penaltiesTime;
        private readonly byte _numPenalties;
        private readonly byte _numTyreStints;
        private readonly TyreStintsDataBuffer _tyreStintsActual;
        private readonly TyreStintsDataBuffer _tyreStintsVisual;
        private readonly TyreStintsDataBuffer _tyreStintsEndLaps;

        /// <summary>
        /// The finishing position of the car.
        /// </summary>
        public byte Position => _position;

        /// <summary>
        /// The number of laps completed by the car.
        /// </summary>
        public byte NumLaps => _numLaps;

        /// <summary>
        /// The starting grid position of the car.
        /// </summary>
        public byte GridPosition => _gridPosition;

        /// <summary>
        /// The number of points scored by the car.
        /// </summary>
        public byte Points => _points;

        /// <summary>
        /// The number of pit stops made by the car.
        /// </summary>
        public byte NumPitStops => _numPitStops;

        /// <summary>
        /// The result status of the car. See <see cref="F125.ResultStatus"/> for possible values.
        /// </summary>
        public ResultStatus ResultStatus => (ResultStatus)_resultStatus;

        /// <summary>
        /// The reason for the result of the car. See <see cref="F125.ResultReason"/> for possible values.
        /// </summary>
        public ResultReason ResultReason => (ResultReason)_resultReason;

        /// <summary>
        /// The best lap time of the car in milliseconds.
        /// </summary>
        public uint BestLapTimeInMS => _bestLapTimeInMS;

        /// <summary>
        /// The total race time of the car in seconds before penalties are applied.
        /// </summary>
        public double TotalRaceTime => _totalRaceTime;

        /// <summary>
        /// The total sum of time penalties applied to the car in seconds.
        /// </summary>
        public byte PenaltiesTime => _penaltiesTime;

        /// <summary>
        /// The number of individual penalties applied to the car.
        /// </summary>
        public byte NumPenalties => _numPenalties;

        /// <summary>
        /// The number of individual stints done by the car.
        /// </summary>
        public byte NumTyreStints => _numTyreStints;

        /// <summary>
        /// The actual tyre compounds used during the session.
        /// </summary>
        public TyreStintsDataBuffer TyreStintsActual => _tyreStintsActual;

        /// <summary>
        /// The visual tyre compounds used during the session.
        /// </summary>
        public TyreStintsDataBuffer TyreStintsVisual => _tyreStintsVisual;

        /// <summary>
        /// The lap number each stint ended on, either by making a pit stop, retiring or finishing.
        /// </summary>
        public TyreStintsDataBuffer TyreStintsEndLaps => _tyreStintsEndLaps;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketFinalClassificationData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _numCars;
        private readonly FinalClassificationDataBuffer _classificationData;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The number of cars shown in the final classification
        /// </summary>
        public byte NumCars => _numCars;

        /// <summary>
        /// The final classification data for each car in the session.
        /// </summary>
        public FinalClassificationDataBuffer ClassificationData => _classificationData;
    }
}