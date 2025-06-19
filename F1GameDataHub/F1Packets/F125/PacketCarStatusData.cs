using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarStatusData
    {
        public byte TractionControl;
        public byte AntiLockBrakes;
        public byte FuelMix;
        public byte FrontBrakeBias;
        public byte PitLimiterStatus;
        public float FuelInTank;
        public float FuelCapacity;
        public float FuelRemainingLaps;
        public ushort MaxRPM;
        public ushort IdleRPM;
        public byte MaxGears;
        public byte DrsAllowed;
        public ushort DrsActivationDistance;
        public byte ActualTyreCompound;
        public byte VisualTyreCompound;
        public byte TyresAgeLaps;
        public sbyte VehicleFIAFlags;
        public float EnginePowerICE;
        public float EnginePowerMGUK;
        public float ErsStoreEnergy;
        public byte ErsDeployMode;
        public float ErsHarvestedThisLapMGUK;
        public float ErsHarvestedThisLapMGUH;
        public float ErsDeployedThisLap;
        public byte NetworkPaused;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarStatusDataBuffer
    {
        private CarStatusData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarStatusData
    {
        public PacketHeader Header;
        public CarStatusDataBuffer CarStatusData;
    }
}