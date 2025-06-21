using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Vector3<T> where T : unmanaged
    {
        private readonly T _x;
        private readonly T _y;
        private readonly T _z;

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public readonly T X => _x;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public readonly T Y => _y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public readonly T Z => _z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CarMotionData
    {
        private readonly Vector3<float> _worldPositionX;
        private readonly Vector3<float> _worldVelocityX;
        private readonly Vector3<short> _worldForwardDirX;
        private readonly Vector3<short> _worldRightDirX;

        private readonly float _gForceLateral;
        private readonly float _gForceLongitudinal;
        private readonly float _gForceVertical;

        private readonly float _yaw;
        private readonly float _pitch;
        private readonly float _roll;

        /// <summary>
        /// The position of the car in world space in meters.
        /// </summary>
        public readonly Vector3<float> WorldPositionX => _worldPositionX;

        /// <summary>
        /// The velocity of the car in world space in meters per second.
        /// </summary>
        public readonly Vector3<float> WorldVelocityX => _worldVelocityX;

        /// <summary>
        /// The forward direction of the car in world space, represented as a normalized vector.
        /// </summary>
        public readonly Vector3<short> WorldForwardDirX => _worldForwardDirX;

        /// <summary>
        /// The right direction of the car in world space, represented as a normalized vector.
        /// </summary>
        public readonly Vector3<short> WorldRightDirX => _worldRightDirX;

        /// <summary>
        /// The lateral G-force experienced by the car.
        /// </summary>
        public readonly float GForceLateral => _gForceLateral;

        /// <summary>
        /// The longitudinal G-force experienced by the car.
        /// </summary>
        public readonly float GForceLongitudinal => _gForceLongitudinal;

        /// <summary>
        /// The vertical G-force experienced by the car.
        /// </summary>
        public readonly float GForceVertical => _gForceVertical;

        /// <summary>
        /// The yaw angle of the car in radians.
        /// </summary>
        public readonly float Yaw => _yaw;

        /// <summary>
        /// The pitch angle of the car in radians.
        /// </summary>
        public readonly float Pitch => _pitch;

        /// <summary>
        /// The roll angle of the car in radians.
        /// </summary>
        public readonly float Roll => _roll;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarMotionDataBuffer
    {
        private CarMotionData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketMotionData : IF1Packet
    {
        public PacketHeader _header;
        public CarMotionDataBuffer _carMotionData;

        /// <inheritdoc/>
        public readonly PacketHeader Header => _header;

        /// <summary>
        /// The motion data for each car in the session.
        /// </summary>
        public readonly CarMotionDataBuffer CarMotionData => _carMotionData;
    }
}