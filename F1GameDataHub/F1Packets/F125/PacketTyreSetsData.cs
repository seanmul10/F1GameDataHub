using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct TyreSetData
    {
        private readonly byte _actualTyreCompound;
        private readonly byte _visualTyreCompound;
        private readonly byte _wear;
        private readonly byte _available;
        private readonly byte _recommendedSession;
        private readonly byte _lifeSpan;
        private readonly byte _usableLife;
        private readonly short _lapDeltaTime;
        private readonly byte _fitted;

        /// <summary>
        /// The actual tyre compound of this set.
        /// </summary>
        public byte ActualTyreCompound => _actualTyreCompound;

        /// <summary>
        /// The visual tyre compound of this set.
        /// </summary>
        public byte VisualTyreCompound => _visualTyreCompound;

        /// <summary>
        /// The percentage of wear on this set.
        /// </summary>
        public byte Wear => _wear;

        /// <summary>
        /// Whether this tyre set is available for use.
        /// </summary>
        public bool Available => _available != 0;

        /// <summary>
        /// The recommended session for this tyre set.
        /// See documentation for the possible values.
        /// </summary>
        public byte RecommendedSession => _recommendedSession;

        /// <summary>
        /// The remaining number of laps this tyre set is expected to last.
        /// </summary>
        public byte LifeSpan => _lifeSpan;

        /// <summary>
        /// The number of laps recommended for this compound.
        /// </summary>
        public byte UsableLife => _usableLife;

        /// <summary>
        /// The delta time in milliseconds for this tyre set compared to the fitted tyre set.
        /// </summary>
        public short LapDeltaTime => _lapDeltaTime;

        /// <summary>
        /// Whether this tyre set is currently fitted to the car.
        /// </summary>
        public bool Fitted => _fitted != 0;
    }

    [InlineArray(PacketConstants.MaxNumTyreSets)]
    public struct TyreSetDataBuffer
    {
        private TyreSetData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketTyreSetsData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _carIdx;
        private readonly TyreSetDataBuffer _tyreSetData;
        private readonly byte _fittedIdx;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The index of the car this data is for.
        /// </summary>
        public byte CarIdx => _carIdx;

        /// <summary>
        /// The data for each tyre set allocated to this car.
        /// </summary>
        public TyreSetDataBuffer TyreSetData => _tyreSetData;

        /// <summary>
        /// The index of the tyre set currently fitted to the car.
        /// </summary>
        public byte FittedIdx => _fittedIdx;
    }
}