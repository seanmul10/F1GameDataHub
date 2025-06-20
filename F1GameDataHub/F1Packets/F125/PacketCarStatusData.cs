using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CarStatusData
    {
        private readonly byte _tractionControl;
        private readonly byte _antiLockBrakes;
        private readonly byte _fuelMix;
        private readonly byte _frontBrakeBias;
        private readonly byte _pitLimiterStatus;
        private readonly float _fuelInTank;
        private readonly float _fuelCapacity;
        private readonly float _fuelRemainingLaps;
        private readonly ushort _maxRPM;
        private readonly ushort _idleRPM;
        private readonly byte _maxGears;
        private readonly byte _drsAllowed;
        private readonly ushort _drsActivationDistance;
        private readonly byte _actualTyreCompound;
        private readonly byte _visualTyreCompound;
        private readonly byte _tyresAgeLaps;
        private readonly sbyte _vehicleFIAFlags;
        private readonly float _enginePowerICE;
        private readonly float _enginePowerMGUK;
        private readonly float _ersStoreEnergy;
        private readonly byte _ersDeployMode;
        private readonly float _ersHarvestedThisLapMGUK;
        private readonly float _ersHarvestedThisLapMGUH;
        private readonly float _ersDeployedThisLap;
        private readonly byte _networkPaused;

        /// <summary>
        /// The level of traction control the car is using. 0 = off, 1 = medium, 2 = high.
        /// </summary>
        public byte TractionControl => _tractionControl;

        /// <summary>
        /// Whether the car is using the anti-lock braking system.
        /// </summary>
        public bool AntiLockBrakes => _antiLockBrakes != 0;

        /// <summary>
        /// The current fuel mix setting of the car. 0 = lean, 1 = standard, 2 = rich, 3 = max.
        /// </summary>
        public byte FuelMix => _fuelMix;

        /// <summary>
        /// The front brake bias setting of the car as a percentage.
        /// </summary>
        public byte FrontBrakeBias => _frontBrakeBias;

        /// <summary>
        /// Whether the pit limiter is currently active.
        /// </summary>
        public bool PitLimiterStatus => _pitLimiterStatus != 0;

        /// <summary>
        /// The current amount of fuel in the tank.
        /// </summary>
        public float FuelInTank => _fuelInTank;

        /// <summary>
        /// The total fuel capacity of the tank.
        /// </summary>
        public float FuelCapacity => _fuelCapacity;

        /// <summary>
        /// The estimated amount of fuel in laps remaining by the end of the race.
        /// </summary>
        public float FuelRemainingLaps => _fuelRemainingLaps;

        /// <summary>
        /// The maximum RPM.
        /// </summary>
        public ushort MaxRPM => _maxRPM;

        /// <summary>
        /// The cars idle RPM.
        /// </summary>
        public ushort IdleRPM => _idleRPM;

        /// <summary>
        /// The maximum number of gears in the car.
        /// </summary>
        public byte MaxGears => _maxGears;

        /// <summary>
        /// Whether the car is currently allowed to activate the DRS.
        /// </summary>
        public bool DrsAllowed => _drsAllowed != 0;

        /// <summary>
        /// The distance in meters from the activation point at which DRS can be activated.
        /// If the value is 0, DRS is not available.
        /// </summary>
        public ushort DrsActivationDistance => _drsActivationDistance;

        /// <summary>
        /// The actual tyre compound fitted to the car.
        /// </summary>
        public byte ActualTyreCompound => _actualTyreCompound;

        /// <summary>
        /// THe visual tyre compound fitted to the car.
        /// </summary>
        public byte VisualTyreCompound => _visualTyreCompound;

        /// <summary>
        /// The number of laps the current set of tyres have been used for.
        /// </summary>
        public byte TyresAgeLaps => _tyresAgeLaps;

        /// <summary>
        /// The flags being shown that apply to this vehicle.
        /// </summary>
        public FlagStatus VehicleFIAFlags => (FlagStatus)_vehicleFIAFlags;

        /// <summary>
        /// The engine power output of the internal combustion engine in watts.
        /// </summary>
        public float EnginePowerICE => _enginePowerICE;

        /// <summary>
        /// The engine power output of the MGU-K in watts.
        /// </summary>
        public float EnginePowerMGUK => _enginePowerMGUK;

        /// <summary>
        /// The amount of energy stored in the ERS system in joules.
        /// </summary>
        public float ErsStoreEnergy => _ersStoreEnergy;

        /// <summary>
        /// The current ERS deployment mode. 0 = none, 1 = medium, 2 = hotlap, 3 = overtake
        /// </summary>
        public byte ErsDeployMode => _ersDeployMode;

        /// <summary>
        /// The energy harvested by the MGU-K during this lap in joules.
        /// </summary>
        public float ErsHarvestedThisLapMGUK => _ersHarvestedThisLapMGUK;

        /// <summary>
        /// The energy harvested by the MGU-H during this lap in joules.
        /// </summary>
        public float ErsHarvestedThisLapMGUH => _ersHarvestedThisLapMGUH;

        /// <summary>
        /// The energy deployed by the ERS system during this lap in joules.
        /// </summary>
        public float ErsDeployedThisLap => _ersDeployedThisLap;

        /// <summary>
        /// Whether the player controlling this car is currently paused in a network session.
        /// </summary>
        public byte NetworkPaused => _networkPaused;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarStatusDataBuffer
    {
        private CarStatusData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketCarStatusData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly CarStatusDataBuffer _carStatusData;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The car status data for each car in the session.
        /// </summary>
        public CarStatusDataBuffer CarStatusData => _carStatusData;
    }
}