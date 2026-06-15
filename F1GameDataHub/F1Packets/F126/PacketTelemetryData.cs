using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F126
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
        private readonly byte _engineTemperature;
        private readonly WheelDataBuffer<float> _tyresPressure;
        private readonly WheelDataBuffer<byte> _surfaceType;

        public ushort Speed => _speed;
        public float Throttle => _throttle;
        public float Steer => _steer;
        public float Brake => _brake;
        public byte Clutch => _clutch;
        public sbyte Gear => _gear;
        public ushort EngineRPM => _engineRPM;
        public bool Drs => _drs != 0;
        public byte RevLightsPercent => _revLightsPercent;
        public ushort RevLightsBitValue => _revLightsBitValue;
        public WheelDataBuffer<ushort> BrakesTemperature => _brakesTemperature;
        public WheelDataBuffer<byte> TyresSurfaceTemperature => _tyresSurfaceTemperature;
        public WheelDataBuffer<byte> TyresInnerTemperature => _tyresInnerTemperature;
        public byte EngineTemperature => _engineTemperature;
        public WheelDataBuffer<float> TyresPressure => _tyresPressure;
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

        public PacketHeader Header => _header;
        public CarTelemetryDataBuffer CarTelemetryData => _carTelemetryData;
        public MfdPanelIndex? MfdPanelIndex =>
            _mfdPanelIndex == 255 ? null : (MfdPanelIndex)_mfdPanelIndex;
        public MfdPanelIndex? MfdPanelIndexSecondaryPlayer =>
            _mfdPanelIndexSecondaryPlayer == 255 ? null : (MfdPanelIndex)_mfdPanelIndexSecondaryPlayer;
        public sbyte SuggestedGear => _suggestedGear;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CarTelemetry2Data
    {
        private readonly byte _activeAeroMode;
        private readonly byte _activeAeroAvailable;
        private readonly ushort _activeAeroActivationDistance;
        private readonly byte _boostAvailable;
        private readonly byte _boostActive;
        private readonly ushort _boostActivationDistance;
        private readonly byte _regulations2026;
        private readonly byte _drivingWrongWay;

        public byte ActiveAeroMode => _activeAeroMode;
        public bool ActiveAeroAvailable => _activeAeroAvailable != 0;
        public ushort ActiveAeroActivationDistance => _activeAeroActivationDistance;
        public bool BoostAvailable => _boostAvailable != 0;
        public bool BoostActive => _boostActive != 0;
        public ushort BoostActivationDistance => _boostActivationDistance;
        public bool Regulations2026 => _regulations2026 != 0;
        public bool DrivingWrongWay => _drivingWrongWay != 0;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarTelemetry2DataBuffer
    {
        private CarTelemetry2Data _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketCarTelemetry2Data : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly CarTelemetry2DataBuffer _carTelemetry2Data;

        public PacketHeader Header => _header;
        public CarTelemetry2DataBuffer CarTelemetry2Data => _carTelemetry2Data;
    }
}
