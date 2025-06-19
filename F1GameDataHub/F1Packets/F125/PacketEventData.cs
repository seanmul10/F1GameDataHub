using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public class EventStringCodes
    {
        public const string SessionStartedEventCode = "SSTA";
        public const string SessionEndedEventCode = "SEND";
        public const string FastestLapEventCode = "FTLP";
        public const string RetirementEventCode = "RTMT";
        public const string DrsEnabledEventCode = "DRSE";
        public const string DrsDisabledEventCode = "DRSD";
        public const string TeamMateInPitsEventCode = "TMPT";
        public const string ChequeredFlagEventCode = "CHQF";
        public const string RaceWinnerEventCode = "RCWN";
        public const string PenaltyEventCode = "PENA";
        public const string SpeedTrapEventCode = "SPTP";
        public const string StartLightsEventCode = "STLG";
        public const string LightsOutEventCode = "LGOT";
        public const string DriveThroughServedEventCode = "DTSV";
        public const string StopGoServedEventCode = "SGSV";
        public const string FlashbackEventCode = "FLBK";
        public const string ButtonStatusEventCode = "BUTN";
        public const string RedFlagEventCode = "RDFL";
        public const string OvertakeEventCode = "OVTK";
        public const string SafetyCarEventCode = "SCAR";
        public const string CollisionEventCode = "COLL";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketEventData
    {
        public PacketHeader Header;

        private byte _eventStringCode0;
        private byte _eventStringCode1;
        private byte _eventStringCode2;
        private byte _eventStringCode3;
        public readonly string EventStringCode => string.Join(string.Empty, (char)_eventStringCode0, (char)_eventStringCode1, (char)_eventStringCode2, (char)_eventStringCode3);

        public EventDataDetails EventDetails;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct EventDataDetails
    {
        // FastestLap
        [FieldOffset(0)] public byte VehicleIdx_FL;
        [FieldOffset(1)] public float LapTime;

        // Retirement
        [FieldOffset(0)] public byte VehicleIdx_RT;
        [FieldOffset(1)] public byte Reason;

        // DRSDisabled
        [FieldOffset(0)] public byte Reason_DRS;

        // TeamMateInPits
        [FieldOffset(0)] public byte VehicleIdx_TMP;

        // RaceWinner
        [FieldOffset(0)] public byte VehicleIdx_RW;

        // Penalty
        [FieldOffset(0)] public byte PenaltyType;
        [FieldOffset(1)] public byte InfringementType;
        [FieldOffset(2)] public byte VehicleIdx_Penalty;
        [FieldOffset(3)] public byte OtherVehicleIdx;
        [FieldOffset(4)] public byte Time;
        [FieldOffset(5)] public byte LapNum;
        [FieldOffset(6)] public byte PlacesGained;

        // SpeedTrap
        [FieldOffset(0)] public byte VehicleIdx_ST;
        [FieldOffset(1)] public float Speed;
        [FieldOffset(5)] public byte IsOverallFastestInSession;
        [FieldOffset(6)] public byte IsDriverFastestInSession;
        [FieldOffset(7)] public byte FastestVehicleIdxInSession;
        [FieldOffset(8)] public float FastestSpeedInSession;

        // StartLights
        [FieldOffset(0)] public byte NumLights;

        // DriveThroughPenaltyServed
        [FieldOffset(0)] public byte VehicleIdx_DTPS;

        // StopGoPenaltyServed
        [FieldOffset(0)] public byte VehicleIdx_SGPS;
        [FieldOffset(1)] public float StopTime;

        // Flashback
        [FieldOffset(0)] public uint FlashbackFrameIdentifier;
        [FieldOffset(4)] public float FlashbackSessionTime;

        // Buttons
        [FieldOffset(0)] public uint ButtonStatus;

        // Overtake
        [FieldOffset(0)] public byte OvertakingVehicleIdx;
        [FieldOffset(1)] public byte BeingOvertakenVehicleIdx;

        // SafetyCar
        [FieldOffset(0)] public byte SafetyCarType;
        [FieldOffset(1)] public byte EventType;

        // Collision
        [FieldOffset(0)] public byte Vehicle1Idx;
        [FieldOffset(1)] public byte Vehicle2Idx;
    }
}