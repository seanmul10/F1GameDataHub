using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TyreSetData
    {
        public byte ActualTyreCompound;
        public byte VisualTyreCompound;
        public byte Wear;
        public byte Available;
        public byte RecommendedSession;
        public byte LifeSpan;
        public byte UsableLife;
        public short LapDeltaTime;
        public byte Fitted;
    }

    [InlineArray(PacketConstants.MaxNumTyreSets)]
    public struct TyreSetDataBuffer
    {
        private TyreSetData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketTyreSetsData
    {
        public PacketHeader Header;
        public byte CarIdx;
        public TyreSetDataBuffer TyreSetData;
        public byte FittedIdx;
    }
}