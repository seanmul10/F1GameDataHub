using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarDamageData
    {
        public TyreDataBuffer<float> TyresWear;
        public TyreDataBuffer<byte> TyresDamage;
        public TyreDataBuffer<byte> BrakesDamage;
        public TyreDataBuffer<byte> TyreBlisters;
        public byte FrontLeftWingDamage;
        public byte FrontRightWingDamage;
        public byte RearWingDamage;
        public byte FloorDamage;
        public byte DiffuserDamage;
        public byte SidepodDamage;
        public byte DrsFault;
        public byte ErsFault;
        public byte GearBoxDamage;
        public byte EngineDamage;
        public byte EngineMGUHWear;
        public byte EngineESWear;
        public byte EngineCEWear;
        public byte EngineICEWear;
        public byte EngineMGUKWear;
        public byte EngineTCWear;
        public byte EngineBlown;
        public byte EngineSeized;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarDamageDataBuffer
    {
        private CarDamageData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarDamageData
    {
        public PacketHeader Header;
        public CarDamageDataBuffer CarDamageData;
    }
}