using System.Runtime.InteropServices;
using System.Text;

namespace F1Packets.F125
{
    public interface IEventData
    {
        /// <summary>
        /// The string code representing the type of event.
        /// </summary>
        string EventStringCode { get; }
    }

    /// <summary>
    /// The string codes for the various event types sent in a <see cref="PacketEventData"/>.
    /// </summary>
    public class EventStringCodes
    {
        /// <summary>
        /// Sent when the session starts.
        /// </summary>
        public const string SessionStartedEventCode = "SSTA";

        /// <summary>
        /// Sent when the session ends.
        /// </summary>
        public const string SessionEndedEventCode = "SEND";

        /// <summary>
        /// Sent when a driver achieves the fastest lap in the session.
        /// </summary>
        public const string FastestLapEventCode = "FTLP";

        /// <summary>
        /// Sent when a driver retires from the session.
        /// </summary>
        public const string RetirementEventCode = "RTMT";

        /// <summary>
        /// Sent when race control enables the drag reduction system.
        /// </summary>
        public const string DrsEnabledEventCode = "DRSE";

        /// <summary>
        /// Sent when race control disables the drag reduction system.
        /// </summary>
        public const string DrsDisabledEventCode = "DRSD";

        /// <summary>
        /// Sent when the teammate of the player car enters the pits.
        /// </summary>
        public const string TeamMateInPitsEventCode = "TMPT";

        /// <summary>
        /// Sent when the chequered flag has been waved.
        /// </summary>
        public const string ChequeredFlagEventCode = "CHQF";

        /// <summary>
        /// Sent when the race has been won.
        /// </summary>
        public const string RaceWinnerEventCode = "RCWN";

        /// <summary>
        /// Sent when a penalty has been issued to a driver.
        /// </summary>
        public const string PenaltyEventCode = "PENA";

        /// <summary>
        /// Sent when the speed trap speed has been triggered.
        /// </summary>
        public const string SpeedTrapEventCode = "SPTP";

        /// <summary>
        /// Sent when each start light is lit.
        /// </summary>
        public const string StartLightsEventCode = "STLG";

        /// <summary>
        /// Sent when the start lights are extinguished.
        /// </summary>
        public const string LightsOutEventCode = "LGOT";

        /// <summary>
        /// Sent when a drive-through penalty is served by a driver.
        /// </summary>
        public const string DriveThroughServedEventCode = "DTSV";

        /// <summary>
        /// Sent when a stop-go penalty is served by a driver.
        /// </summary>
        public const string StopGoServedEventCode = "SGSV";

        /// <summary>
        /// Sent when a flashback is triggered by the player.
        /// </summary>
        public const string FlashbackEventCode = "FLBK";

        /// <summary>
        /// Sent when a driver input button status changes.
        /// </summary>
        public const string ButtonStatusEventCode = "BUTN";

        /// <summary>
        /// Sent when the session is red flagged.
        /// </summary>
        public const string RedFlagEventCode = "RDFL";

        /// <summary>
        /// Sent when an on-track overtake occurs.
        /// </summary>
        public const string OvertakeEventCode = "OVTK";

        /// <summary>
        /// Sent when race control signals a safety car, virtual safety car, or formation lap message.
        /// </summary>
        public const string SafetyCarEventCode = "SCAR";

        /// <summary>
        /// Sent when a collision occurs between two vehicles.
        /// </summary>
        public const string CollisionEventCode = "COLL";
    }

    public enum ResultReason
    {
        Invalid = 0,
        Retired = 1,
        Finished = 2,
        TerminalDamage = 3,
        Inactive = 4,
        NotClassified = 5,
        BlackFlagged = 6,
        RedFlagged = 7,
        MechanicalFailure = 8,
        SessionSkipped = 9,
        SessionSimulated = 10,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketEventData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _eventStringCode0;
        private readonly byte _eventStringCode1;
        private readonly byte _eventStringCode2;
        private readonly byte _eventStringCode3;
        private readonly EventDataDetails _eventDetails;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The details related to the event, depending on the <see cref="EventStringCode"/>.
        /// </summary>
        public IEventData EventDetails
        {
            get
            {
                var eventStringCode = Encoding.ASCII.GetString([_eventStringCode0, _eventStringCode1, _eventStringCode2, _eventStringCode3]);
                return eventStringCode switch
                {
                    EventStringCodes.FastestLapEventCode => _eventDetails.FastestLap,
                    EventStringCodes.RetirementEventCode => _eventDetails.Retirement,
                    EventStringCodes.DrsDisabledEventCode => _eventDetails.DrsDisabled,
                    EventStringCodes.TeamMateInPitsEventCode => _eventDetails.TeamMateInPits,
                    EventStringCodes.RaceWinnerEventCode => _eventDetails.RaceWinner,
                    EventStringCodes.PenaltyEventCode => _eventDetails.Penalty,
                    EventStringCodes.SpeedTrapEventCode => _eventDetails.SpeedTrap,
                    EventStringCodes.StartLightsEventCode => _eventDetails.StartLights,
                    EventStringCodes.DriveThroughServedEventCode => _eventDetails.DriveThroughServed,
                    EventStringCodes.StopGoServedEventCode => _eventDetails.StopGoServed,
                    EventStringCodes.FlashbackEventCode => _eventDetails.Flashback,
                    EventStringCodes.ButtonStatusEventCode => _eventDetails.ButtonStatus,
                    EventStringCodes.OvertakeEventCode => _eventDetails.Overtake,
                    EventStringCodes.SafetyCarEventCode => _eventDetails.SafetyCar,
                    EventStringCodes.CollisionEventCode => _eventDetails.Collision,
                    _ => throw new InvalidOperationException($"Unknown event string code: {eventStringCode}")
                };
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct EventDataDetails
    {
        [FieldOffset(0)] public FastestLapData FastestLap;
        [FieldOffset(0)] public RetirementData Retirement;
        [FieldOffset(0)] public DrsDisabledData DrsDisabled;
        [FieldOffset(0)] public TeamMateInPitsData TeamMateInPits;
        [FieldOffset(0)] public RaceWinnerData RaceWinner;
        [FieldOffset(0)] public PenaltyData Penalty;
        [FieldOffset(0)] public SpeedTrapData SpeedTrap;
        [FieldOffset(0)] public StartLightsData StartLights;
        [FieldOffset(0)] public DriveThroughServedData DriveThroughServed;
        [FieldOffset(0)] public StopGoServedData StopGoServed;
        [FieldOffset(0)] public FlashbackData Flashback;
        [FieldOffset(0)] public ButtonStatusData ButtonStatus;
        [FieldOffset(0)] public OvertakeData Overtake;
        [FieldOffset(0)] public SafetyCarData SafetyCar;
        [FieldOffset(0)] public CollisionData Collision;
    }

    // === Individual Event Structs ===

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct FastestLapData : IEventData
    {
        private readonly byte _vehicleIdx;
        private readonly float _lapTime;

        /// <summary>
        /// The vehicle index of the car that achieved the fastest lap.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <summary>
        /// The lap time of the fastest lap in seconds.
        /// </summary>
        public float LapTime => _lapTime;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.FastestLapEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct RetirementData : IEventData
    {
        private readonly byte _vehicleIdx;
        private readonly byte _reason;

        /// <summary>
        /// The vehicle index of the car that retired.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <summary>
        /// The reason for retirement. See <see cref="ResultReason"/> for possible values.
        /// </summary>
        public ResultReason Reason => (ResultReason)_reason;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.RetirementEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct DrsDisabledData : IEventData
    {
        private readonly byte _reason;

        /// <summary>
        /// The reason for DRS being disabled. 0 = Wet track, 1 = Safety car deployed, 2 = Red flag, 3 = Min lap not reached
        /// </summary>
        public byte Reason => _reason;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.DrsDisabledEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct TeamMateInPitsData : IEventData
    {
        private readonly byte _vehicleIdx;

        /// <summary>
        /// The vehicle index of the player's teammate
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.TeamMateInPitsEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct RaceWinnerData : IEventData
    {
        private readonly byte _vehicleIdx;

        /// <summary>
        /// The vehicle index of the winner.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.RaceWinnerEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PenaltyData : IEventData
    {
        private readonly byte _penaltyType;
        private readonly byte _infringementType;
        private readonly byte _vehicleIdx;
        private readonly byte _otherVehicleIdx;
        private readonly byte _time;
        private readonly byte _lapNum;
        private readonly byte _placesGained;

        /// <summary>
        /// The type of penalty issued.
        /// See documentation for the possible values.
        /// </summary>
        public byte PenaltyType => _penaltyType;

        /// <summary>
        /// The type of infringement that led to the penalty.
        /// See documentation for the possible values.
        /// </summary>
        public byte InfringementType => _infringementType;

        /// <summary>
        /// The vehicle index of the car that received the penalty.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <summary>
        /// The vehicle index of the other car involved in the penalty, if applicable.
        /// </summary>
        public byte OtherVehicleIdx => _otherVehicleIdx;

        /// <summary>
        /// The time gained, or time spent during the infringement, in seconds.
        /// </summary>
        public byte Time => _time;

        /// <summary>
        /// The lap number the infringement occurred on.
        /// </summary>
        public byte LapNum => _lapNum;

        /// <summary>
        /// The number of places gained as a result of the infringement.
        /// </summary>
        public byte PlacesGained => _placesGained;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.PenaltyEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct SpeedTrapData : IEventData
    {
        private readonly byte _vehicleIdx;
        private readonly float _speed;
        private readonly byte _isOverallFastestInSession;
        private readonly byte _isDriverFastestInSession;
        private readonly byte _fastestVehicleIdxInSession;
        private readonly float _fastestSpeedInSession;

        /// <summary>
        /// The vehicle index of the car that passed through the speed trap.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <summary>
        /// The speed recorded through the speed trap in kilometers per hour.
        /// </summary>
        public float Speed => _speed;

        /// <summary>
        /// Whether this is the overall fastest speed recorded in the session.
        /// </summary>
        public bool IsOverallFastestInSession => _isOverallFastestInSession != 0;

        /// <summary>
        /// Whether this is the personal fastest speed recorded by the driver in the session.
        /// </summary>
        public bool IsDriverFastestInSession => _isDriverFastestInSession != 0;

        /// <summary>
        /// The index of the vehicle that holds the fastest recorded speed through the speed trap in the session.
        /// </summary>
        public byte FastestVehicleIdxInSession => _fastestVehicleIdxInSession;

        /// <summary>
        /// The recorded speed of the fastest vehicle in the session through the speed trap in kilometers per hour.
        /// </summary>
        public float FastestSpeedInSession => _fastestSpeedInSession;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.SpeedTrapEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct StartLightsData : IEventData
    {
        private readonly byte _numLights;

        /// <summary>
        /// The number of lights that are currently lit.
        /// </summary>
        public byte NumLights => _numLights;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.StartLightsEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct DriveThroughServedData : IEventData
    {
        private readonly byte _vehicleIdx;

        /// <summary>
        /// The vehicle index of the car that served the drive-through penalty.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.DriveThroughServedEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct StopGoServedData : IEventData
    {
        private readonly byte _vehicleIdx;
        private readonly float _stopTime;

        /// <summary>
        /// The vehicle index of the car that served the stop-go penalty.
        /// </summary>
        public byte VehicleIdx => _vehicleIdx;

        /// <summary>
        /// The time spent serving the stop-go penalty in seconds.
        /// </summary>
        public float StopTime => _stopTime;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.StopGoServedEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct FlashbackData : IEventData
    {
        private readonly uint _flashbackFrameIdentifier;
        private readonly float _flashbackSessionTime;

        /// <summary>
        /// The identifier of the frame that was flashed back to.
        /// </summary>
        public uint FlashbackFrameIdentifier => _flashbackFrameIdentifier;

        /// <summary>
        /// The session time that was flashed back to.
        /// </summary>
        public float FlashbackSessionTime => _flashbackSessionTime;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.FlashbackEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ButtonStatusData : IEventData
    {
        private readonly uint _buttonStatus;

        /// <summary>
        /// The bit flags representing the inputs currently being pressed by the player.
        /// </summary>
        public uint ButtonStatus => _buttonStatus;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.ButtonStatusEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct OvertakeData : IEventData
    {
        private readonly byte _overtakingVehicleIdx;
        private readonly byte _beingOvertakenVehicleIdx;

        /// <summary>
        /// The vehicle index of the car that is overtaking another vehicle.
        /// </summary>
        public byte OvertakingVehicleIdx => _overtakingVehicleIdx;

        /// <summary>
        /// The vehicle index of the car that has been overtaken.
        /// </summary>
        public byte BeingOvertakenVehicleIdx => _beingOvertakenVehicleIdx;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.OvertakeEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct SafetyCarData : IEventData
    {
        private readonly byte _safetyCarType;
        private readonly byte _eventType;

        /// <summary>
        /// The safety car type. 0 = None, 1 = Full Safety Car, 2 = Virtual Safety Car, 3 = Formation Lap.
        /// </summary>
        public byte SafetyCarType => _safetyCarType;

        /// <summary>
        /// The event type. 0 = Deployed, 1 = In this lap, 2 = In pits, 3 = Green flag
        /// </summary>
        public byte EventType => _eventType;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.SafetyCarEventCode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct CollisionData : IEventData
    {
        private readonly byte _vehicle1Idx;
        private readonly byte _vehicle2Idx;

        /// <summary>
        /// The index of the first vehicle involved in the collision.
        /// </summary>
        public byte Vehicle1Idx => _vehicle1Idx;

        /// <summary>
        /// The index of the second vehicle involved in the collision.
        /// </summary>
        public byte Vehicle2Idx => _vehicle2Idx;

        /// <inheritdoc/>
        public string EventStringCode => EventStringCodes.CollisionEventCode;
    }
}