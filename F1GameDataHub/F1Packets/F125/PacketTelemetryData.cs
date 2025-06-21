using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public enum MfdPanelIndex
    {
        CarSetup = 0,
        Pits = 1,
        Damage = 2,
        Engine = 3,
        Temperatures = 4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CarTelemetryData
    {
        private readonly ushort _speed;
        private readonly float _throttle;
        private readonly float _steer;
        private readonly float _brake;
        private readonly byte _clutch;
        private readonly sbyte _gear;
        private readonly ushort _engineRPM;
        private readonly byte _drs;
        private readonly byte _revLightsPercent;
        private readonly ushort _revLightsBitValue;
        private readonly WheelDataBuffer<ushort> _brakesTemperature;
        private readonly WheelDataBuffer<byte> _tyresSurfaceTemperature;
        private readonly WheelDataBuffer<byte> _tyresInnerTemperature;
        private readonly ushort _engineTemperature;
        private readonly WheelDataBuffer<float> _tyresPressure;
        private readonly WheelDataBuffer<byte> _surfaceType;

        /// <summary>
        /// The speed of the car in kilometers per hour.
        /// </summary>
        public ushort Speed => _speed;

        /// <summary>
        /// The amount of throttle applied by the driver, ranging from 0.0 (no throttle) to 1.0 (full throttle).
        /// </summary>
        public float Throttle => _throttle;

        /// <summary>
        /// The amount of steering applied by the driver, ranging from -1.0 (full lock left) to 1.0 (full lock right).
        /// </summary>
        public float Steer => _steer;

        /// <summary>
        /// The amount of brake applied by the driver, ranging from 0.0 (no brake) to 1.0 (full brake).
        /// </summary>
        public float Brake => _brake;

        /// <summary>
        /// The amount of clutch applied by the driver, ranging from 0 (no clutch) to 100 (full clutch).
        /// </summary>
        public byte Clutch => _clutch;

        /// <summary>
        /// The current gear selected. 1-8 for forward gears, 0 for neutral, -1 for reverse.
        /// </summary>
        public sbyte Gear => _gear;

        /// <summary>
        /// The current engine RPM.
        /// </summary>
        public ushort EngineRPM => _engineRPM;

        /// <summary>
        /// Whether the DRS is activated.
        /// </summary>
        public bool Drs => _drs != 0;

        /// <summary>
        /// The rev light indicator percentage.
        /// </summary>
        public byte RevLightsPercent => _revLightsPercent;

        /// <summary>
        /// The bit value of the rev lights, where each bit represents a light being on or off.
        /// Bit 0 = leftmost LED, Bit 14 = rightmost LED.
        /// </summary>
        public ushort RevLightsBitValue => _revLightsBitValue;

        /// <summary>
        /// The temperature of each brake in degrees Celsius.
        /// </summary>
        public WheelDataBuffer<ushort> BrakesTemperature => _brakesTemperature;

        /// <summary>
        /// The surface temperature of each tyre in degrees Celsius.
        /// </summary>
        public WheelDataBuffer<byte> TyresSurfaceTemperature => _tyresSurfaceTemperature;

        /// <summary>
        /// The carcass temperature of each tyre in degrees Celsius.
        /// </summary>
        public WheelDataBuffer<byte> TyresInnerTemperature => _tyresInnerTemperature;

        /// <summary>
        /// The temperature of the engine in degrees Celsius.
        /// </summary>
        public ushort EngineTemperature => _engineTemperature;

        /// <summary>
        /// The pressure of each tyre in PSI.
        /// </summary>
        public WheelDataBuffer<float> TyresPressure => _tyresPressure;

        /// <summary>
        /// The surface each tyre is currently driving on.
        /// See the documentation for the surface type for each ID.
        /// </summary>
        public WheelDataBuffer<byte> SurfaceType => _surfaceType;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarTelemetryDataBuffer
    {
        private CarTelemetryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketCarTelemetryData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly CarTelemetryDataBuffer _carTelemetryData;
        private readonly byte _mfdPanelIndex;
        private readonly byte _mfdPanelIndexSecondaryPlayer;
        private readonly sbyte _suggestedGear;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The telemetry data for each car in the session.
        /// </summary>
        public CarTelemetryDataBuffer CarTelemetryData => _carTelemetryData;

        /// <summary>
        /// The index of the MFD panel currently being displayed by the player.
        /// </summary>
        public MfdPanelIndex? MfdPanelIndex =>
            _mfdPanelIndexSecondaryPlayer == 255 ? null : (MfdPanelIndex)_mfdPanelIndexSecondaryPlayer;

        /// <summary>
        /// The index of the MFD panel currently being displayed by the secondary player, if applicable.
        /// </summary>
        public MfdPanelIndex? MfdPanelIndexSecondaryPlayer =>
            _mfdPanelIndex == 255 ? null : (MfdPanelIndex)_mfdPanelIndex;

        /// <summary>
        /// The games suggested gear for the player, or 0 if turned off.
        /// </summary>
        public sbyte SuggestedGear => _suggestedGear;
    }
}