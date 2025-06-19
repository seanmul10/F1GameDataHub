using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarMotionData
    {
        public float WorldPositionX;
        public float WorldPositionY;
        public float WorldPositionZ;

        public float WorldVelocityX;
        public float WorldVelocityY;
        public float WorldVelocityZ;

        public short WorldForwardDirX;
        public short WorldForwardDirY;
        public short WorldForwardDirZ;

        public short WorldRightDirX;
        public short WorldRightDirY;
        public short WorldRightDirZ;

        public float GForceLateral;
        public float GForceLongitudinal;
        public float GForceVertical;

        public float Yaw;
        public float Pitch;
        public float Roll;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarMotionDataBuffer
    {
        private CarMotionData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketMotionData
    {
        public PacketHeader Header;
        public CarMotionDataBuffer CarMotionData;
    }
}