using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketMotionExData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly TyreDataBuffer<float> _suspensionPosition;
        private readonly TyreDataBuffer<float> _suspensionVelocity;
        private readonly TyreDataBuffer<float> _suspensionAcceleration;
        private readonly TyreDataBuffer<float> _wheelSpeed;
        private readonly TyreDataBuffer<float> _wheelSlipRatio;
        private readonly TyreDataBuffer<float> _wheelSlipAngle;
        private readonly TyreDataBuffer<float> _wheelLatForce;
        private readonly TyreDataBuffer<float> _wheelLongForce;
        private readonly float _heightOfCOGAboveGround;
        private readonly Vector3<float> _localVelocity;
        private readonly Vector3<float> _angularVelocity;
        private readonly Vector3<float> _angularAcceleration;
        private readonly float _frontWheelsAngle;
        private readonly TyreDataBuffer<float> _wheelVertForce;
        private readonly float _frontAeroHeight;
        private readonly float _rearAeroHeight;
        private readonly float _frontRollAngle;
        private readonly float _rearRollAngle;
        private readonly float _chassisYaw;
        private readonly float _chassisPitch;
        private readonly TyreDataBuffer<float> _wheelCamber;
        private readonly TyreDataBuffer<float> _wheelCamberGain;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The position of the suspension for each wheel in meters.
        /// </summary>
        public TyreDataBuffer<float> SuspensionPosition => _suspensionPosition;

        /// <summary>
        /// The velocity of the suspension for each wheel in meters per second.
        /// </summary>
        public TyreDataBuffer<float> SuspensionVelocity => _suspensionVelocity;

        /// <summary>
        /// The acceleration of the suspension for each wheel in meters per second squared.
        /// </summary>
        public TyreDataBuffer<float> SuspensionAcceleration => _suspensionAcceleration;

        /// <summary>
        /// The speed of each wheel in meters per second.
        /// </summary>
        public TyreDataBuffer<float> WheelSpeed => _wheelSpeed;

        /// <summary>
        /// The wheel slip ratio for each wheel, representing the difference between the wheel speed and the vehicle speed.
        /// </summary>
        public TyreDataBuffer<float> WheelSlipRatio => _wheelSlipRatio;

        /// <summary>
        /// The wheel slip angle for each wheel in radians, indicating the angle between the wheel's direction and its actual path.
        /// </summary>
        public TyreDataBuffer<float> WheelSlipAngle => _wheelSlipAngle;

        /// <summary>
        /// The lateral force on each wheel in newtons.
        /// </summary>
        public TyreDataBuffer<float> WheelLatForce => _wheelLatForce;

        /// <summary>
        /// The longitudinal force on each wheel in newtons.
        /// </summary>
        public TyreDataBuffer<float> WheelLongForce => _wheelLongForce;

        /// <summary>
        /// The height of the center of gravity above the ground in meters.
        /// </summary>
        public float HeightOfCOGAboveGround => _heightOfCOGAboveGround;

        /// <summary>
        /// The velocity of the car in local space in meters per second.
        /// </summary>
        public Vector3<float> LocalVelocity => _localVelocity;

        /// <summary>
        /// The angular velocity of the car in radians per second.
        /// </summary>
        public Vector3<float> AngularVelocity => _angularVelocity;

        /// <summary>
        /// The angular acceleration of the car in radians per second squared.
        /// </summary>
        public Vector3<float> AngularAcceleration => _angularAcceleration;

        /// <summary>
        /// Gets the current angle of the front wheels in radians.
        /// </summary>
        public float FrontWheelsAngle => _frontWheelsAngle;

        /// <summary>
        /// The vertical force on each wheel in newtons.
        /// </summary>
        public TyreDataBuffer<float> WheelVertForce => _wheelVertForce;

        /// <summary>
        /// The height of the front plank edge above the ground in meters.
        /// </summary>
        public float FrontAeroHeight => _frontAeroHeight;

        /// <summary>
        /// The height of the rear plank edge above the ground in meters.
        /// </summary>
        public float RearAeroHeight => _rearAeroHeight;

        /// <summary>
        /// The roll angle of the front suspension in radians.
        /// </summary>
        public float FrontRollAngle => _frontRollAngle;

        /// <summary>
        /// The roll angle of the rear suspension in radians.
        /// </summary>
        public float RearRollAngle => _rearRollAngle;

        /// <summary>
        /// The yaw angle of the chassis relative to the direction of travel in radians.
        /// </summary>
        public float ChassisYaw => _chassisYaw;

        /// <summary>
        /// The pich angle of the chassis relative to the direction of travel in radians.
        /// </summary>
        public float ChassisPitch => _chassisPitch;

        /// <summary>
        /// The camber of each wheel in radians.
        /// </summary>
        public TyreDataBuffer<float> WheelCamber => _wheelCamber;

        /// <summary>
        /// The camber gain of each wheel in radians, representing the difference between the active camber and dynamic camber.
        /// </summary>
        public TyreDataBuffer<float> WheelCamberGain => _wheelCamberGain;
    }
}