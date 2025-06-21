using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CarSetupData
    {
        private readonly byte _frontWing;
        private readonly byte _rearWing;
        private readonly byte _onThrottle;
        private readonly byte _offThrottle;
        private readonly float _frontCamber;
        private readonly float _rearCamber;
        private readonly float _frontToe;
        private readonly float _rearToe;
        private readonly byte _frontSuspension;
        private readonly byte _rearSuspension;
        private readonly byte _frontAntiRollBar;
        private readonly byte _rearAntiRollBar;
        private readonly byte _frontSuspensionHeight;
        private readonly byte _rearSuspensionHeight;
        private readonly byte _brakePressure;
        private readonly byte _brakeBias;
        private readonly byte _engineBraking;
        private readonly WheelDataBuffer<float> _tyrePressure;
        private readonly byte _ballast;
        private readonly float _fuelLoad;

        /// <summary>
        /// The front wing angle setting.
        /// </summary>
        public byte FrontWing => _frontWing;

        /// <summary>
        /// The rear wing angle setting.
        /// </summary>
        public byte RearWing => _rearWing;

        /// <summary>
        /// The differencial adjustment for when the car is on throttle.
        /// </summary>
        public byte OnThrottle => _onThrottle;

        /// <summary>
        /// The differencial adjustment for when the car is off throttle.
        /// </summary>
        public byte OffThrottle => _offThrottle;

        /// <summary>
        /// The front camber angle of the suspension geometry.
        /// </summary>
        public float FrontCamber => _frontCamber;

        /// <summary>
        /// The rear camber angle of the suspension geometry.
        /// </summary>
        public float RearCamber => _rearCamber;

        /// <summary>
        /// The front toe angle of the suspension geometry.
        /// </summary>
        public float FrontToe => _frontToe;

        /// <summary>
        /// The rear toe angle of the suspension geometry.
        /// </summary>
        public float RearToe => _rearToe;

        /// <summary>
        /// The front suspension setting.
        /// </summary>
        public byte FrontSuspension => _frontSuspension;

        /// <summary>
        /// The rear suspension setting.
        /// </summary>
        public byte RearSuspension => _rearSuspension;

        /// <summary>
        /// The front anti-roll bar setting.
        /// </summary>
        public byte FrontAntiRollBar => _frontAntiRollBar;

        /// <summary>
        /// The rear anti-roll bar setting.
        /// </summary>
        public byte RearAntiRollBar => _rearAntiRollBar;

        /// <summary>
        /// The front ride height setting.
        /// </summary>
        public byte FrontSuspensionHeight => _frontSuspensionHeight;

        /// <summary>
        /// The rear ride height setting.
        /// </summary>
        public byte RearSuspensionHeight => _rearSuspensionHeight;

        /// <summary>
        /// The brake pressure as a percentage.
        /// </summary>
        public byte BrakePressure => _brakePressure;

        /// <summary>
        /// The brake bias setting as a percentage.
        /// </summary>
        public byte BrakeBias => _brakeBias;

        /// <summary>
        /// The engine braking setting as a percentage.
        /// </summary>
        public byte EngineBraking => _engineBraking;

        /// <summary>
        /// The set initial tyre pressures for each tyre.
        /// </summary>
        public WheelDataBuffer<float> TyrePressure => _tyrePressure;

        /// <summary>
        /// The ballast setting.
        /// </summary>
        public byte Ballast => _ballast;

        /// <summary>
        /// The initial fuel load of the car.
        /// </summary>
        public float FuelLoad => _fuelLoad;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct CarSetupDataBuffer
    {
        private CarSetupData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketCarSetupsData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly CarSetupDataBuffer _carSetupDataBuffer;
        private readonly float _nextFrontWingValue;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The car setup data for each car in the session.
        /// </summary>
        public CarSetupDataBuffer CarSetupDataBuffer => _carSetupDataBuffer;

        /// <summary>
        /// The angle the front wing will be adjusted to at the next pit stop.
        /// Only relevant to the player.
        /// </summary>
        public float NextFrontWingValue => _nextFrontWingValue;
    }
}