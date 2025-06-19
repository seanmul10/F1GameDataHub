using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarTelemetryData
    {
        public ushort Speed;
        public float Throttle;
        public float Steer;
        public float Brake;
        public byte Clutch;
        public sbyte Gear;
        public ushort EngineRPM;
        public byte Drs;
        public byte RevLightsPercent;
        public ushort RevLightsBitValue;
        public TyreDataBuffer<ushort> BrakesTemperature;
        public TyreDataBuffer<byte> TyresSurfaceTemperature;
        public TyreDataBuffer<byte> TyresInnerTemperature;
        public ushort EngineTemperature;
        public TyreDataBuffer<float> TyresPressure;
        public TyreDataBuffer<byte> SurfaceType;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarTelemetryDataBuffer
    {
        private CarTelemetryData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarTelemetryData
    {
        public PacketHeader Header;
        public CarTelemetryDataBuffer CarTelemetryData;
        public byte MfdPanelIndex;
        public byte MfdPanelIndexSecondaryPlayer;
        public sbyte SuggestedGear;
    }
}