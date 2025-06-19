using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarSetupData
    {
        public byte FrontWing;
        public byte RearWing;
        public byte OnThrottle;
        public byte OffThrottle;

        public float FrontCamber;
        public float RearCamber;
        public float FrontToe;
        public float RearToe;

        public byte FrontSuspension;
        public byte RearSuspension;
        public byte FrontAntiRollBar;
        public byte RearAntiRollBar;

        public byte FrontSuspensionHeight;
        public byte RearSuspensionHeight;
        public byte BrakePressure;
        public byte BrakeBias;
        public byte EngineBraking;

        public float RearLeftTyrePressure;
        public float RearRightTyrePressure;
        public float FrontLeftTyrePressure;
        public float FrontRightTyrePressure;

        public byte Ballast;
        public float FuelLoad;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarSetupDataBuffer
    {
        private CarSetupData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarSetupsData
    {
        public PacketHeader Header;
        public CarSetupDataBuffer CarSetupDataBuffer;
        public float NextFrontWingValue;
    }
}