using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketMotionExData
    {
        public PacketHeader Header;

        // Arrays for 4 wheels in order: RL, RR, FL, FR
        public TyreDataBuffer<float> SuspensionPosition;
        public TyreDataBuffer<float> SuspensionVelocity;
        public TyreDataBuffer<float> SuspensionAcceleration;
        public TyreDataBuffer<float> WheelSpeed;
        public TyreDataBuffer<float> WheelSlipRatio;
        public TyreDataBuffer<float> WheelSlipAngle;
        public TyreDataBuffer<float> WheelLatForce;
        public TyreDataBuffer<float> WheelLongForce;

        public float HeightOfCOGAboveGround;

        public float LocalVelocityX;
        public float LocalVelocityY;
        public float LocalVelocityZ;

        public float AngularVelocityX;
        public float AngularVelocityY;
        public float AngularVelocityZ;

        public float AngularAccelerationX;
        public float AngularAccelerationY;
        public float AngularAccelerationZ;

        public float FrontWheelsAngle;

        public TyreDataBuffer<float> WheelVertForce;

        public float FrontAeroHeight;
        public float RearAeroHeight;

        public float FrontRollAngle;
        public float RearRollAngle;

        public float ChassisYaw;
        public float ChassisPitch;

        public TyreDataBuffer<float> WheelCamber;
        public TyreDataBuffer<float> WheelCamberGain;
    }
}