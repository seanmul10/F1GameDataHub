using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarDamageData
    {
        private WheelDataBuffer<float> _tyresWear;
        private WheelDataBuffer<byte> _tyresDamage;
        private WheelDataBuffer<byte> _brakesDamage;
        private WheelDataBuffer<byte> _tyreBlisters;
        private readonly byte _frontLeftWingDamage;
        private readonly byte _frontRightWingDamage;
        private readonly byte _rearWingDamage;
        private readonly byte _floorDamage;
        private readonly byte _diffuserDamage;
        private readonly byte _sidepodDamage;
        private readonly byte _drsFault;
        private readonly byte _ersFault;
        private readonly byte _gearBoxDamage;
        private readonly byte _engineDamage;
        private readonly byte _engineMGUHWear;
        private readonly byte _engineESWear;
        private readonly byte _engineCEWear;
        private readonly byte _engineICEWear;
        private readonly byte _engineMGUKWear;
        private readonly byte _engineTCWear;
        private readonly byte _engineBlown;
        private readonly byte _engineSeized;

        /// <summary>
        /// The percentage of wear for each tyre.
        /// </summary>
        public readonly WheelDataBuffer<float> TyresWear => _tyresWear;

        /// <summary>
        /// The percentage of damage to each tyre.
        /// </summary>
        public readonly WheelDataBuffer<byte> TyresDamage => _tyresDamage;

        /// <summary>
        /// The percentage of damage to each of the brakes.
        /// </summary>
        public readonly WheelDataBuffer<byte> BrakesDamage => _brakesDamage;

        /// <summary>
        /// The percentage of blistering on each tyre.
        /// </summary>
        public readonly WheelDataBuffer<byte> TyreBlisters => _tyreBlisters;

        /// <summary>
        /// The percentage of damage to the front left wing.
        /// </summary>
        public readonly byte FrontLeftWingDamage => _frontLeftWingDamage;

        /// <summary>
        /// The percentage of damage to the front right wing.
        /// </summary>
        public readonly byte FrontRightWingDamage => _frontRightWingDamage;

        /// <summary>
        /// The percentage of damage to the rear wing.
        /// </summary>
        public readonly byte RearWingDamage => _rearWingDamage;

        /// <summary>
        /// The percentage of damage to the floor the car.
        /// </summary>
        public readonly byte FloorDamage => _floorDamage;

        /// <summary>
        /// The percentage of damage to the rear diffuser.
        /// </summary>
        public readonly byte DiffuserDamage => _diffuserDamage;

        /// <summary>
        /// The percentage of damage to the sidepods.
        /// </summary>
        public readonly byte SidepodDamage => _sidepodDamage;

        /// <summary>
        /// Whether there is currently a fault with the DRS system.
        /// </summary>
        public readonly bool DrsFault => _drsFault != 0;

        /// <summary>
        /// Whether there is currently a fault with the ERS system.
        /// </summary>
        public readonly bool ErsFault => _ersFault != 0;

        /// <summary>
        /// The percentage of damage to the gearbox.
        /// </summary>
        public readonly byte GearBoxDamage => _gearBoxDamage;

        /// <summary>
        /// The percentage of damage to the engine.
        /// </summary>
        public readonly byte EngineDamage => _engineDamage;

        /// <summary>
        /// The percentage of wear on the MGU-H of the power unit.
        /// </summary>
        public readonly byte EngineMGUHWear => _engineMGUHWear;

        /// <summary>
        /// The percentage of wear on the energy store of the power unit.
        /// </summary>
        public readonly byte EngineESWear => _engineESWear;

        /// <summary>
        /// The percentage of wear on the control electronics of the power unit.
        /// </summary>
        public readonly byte EngineCEWear => _engineCEWear;

        /// <summary>
        /// The percentage of wear on the internal combustion engine of the power unit.
        /// </summary>
        public readonly byte EngineICEWear => _engineICEWear;

        /// <summary>
        /// The percentage of wear on the MGU-K of the power unit.
        /// </summary>
        public readonly byte EngineMGUKWear => _engineMGUKWear;

        /// <summary>
        /// The percentage of wear on the turbocharger of the power unit.
        /// </summary>
        public readonly byte EngineTCWear => _engineTCWear;

        /// <summary>
        /// Whether the engine has blown.
        /// </summary>
        public readonly bool EngineBlown => _engineBlown != 0;

        /// <summary>
        /// Whether the engine has seized.
        /// </summary>
        public readonly bool EngineSeized => _engineSeized != 0;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarDamageDataBuffer
    {
        private CarDamageData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketCarDamageData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly CarDamageDataBuffer _carDamageData;

        /// <inheritdoc/>
        public readonly PacketHeader Header => _header;

        /// <summary>
        /// The car damage data for all cars in the session.
        /// </summary>
        public readonly CarDamageDataBuffer CarDamageData => _carDamageData;
    }
}