using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class SessionPacketConstants
    {
        public const int MaxMarshalsZonePerLap = 21;
        public const int MaxWeatherForecastSamples = 64;
        public const int MaxSessionsInWeekend = 12;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MarshalZone
    {
        public float ZoneStart;  // m_zoneStart
        public sbyte ZoneFlag;   // m_zoneFlag (int8)
    }

    [InlineArray(SessionPacketConstants.MaxMarshalsZonePerLap)]
    public struct MarshalZoneBuffer
    {
        private MarshalZone _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WeatherForecastSample
    {
        public byte SessionType;
        public byte TimeOffset;
        public byte Weather;
        public sbyte TrackTemperature;
        public sbyte TrackTemperatureChange;
        public sbyte AirTemperature;
        public sbyte AirTemperatureChange;
        public byte RainPercentage;
    }

    [InlineArray(SessionPacketConstants.MaxWeatherForecastSamples)]
    public struct WeatherForecastSampleBuffer
    {
        private WeatherForecastSample _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WeekendStructure
    {
        public byte Value;
    }

    [InlineArray(SessionPacketConstants.MaxSessionsInWeekend)]
    public struct WeekendStructureBuffer
    {
        private WeekendStructure _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketSessionData
    {
        public PacketHeader Header;
        public byte Weather;
        public sbyte TrackTemperature;
        public sbyte AirTemperature;
        public byte TotalLaps;
        public ushort TrackLength;
        public byte SessionType;
        public sbyte TrackId;
        public byte Formula;
        public ushort SessionTimeLeft;
        public ushort SessionDuration;
        public byte PitSpeedLimit;
        public byte GamePaused;
        public byte IsSpectating;
        public byte SpectatorCarIndex;
        public byte SliProNativeSupport;
        public byte NumMarshalZones;
        public MarshalZoneBuffer MarshalZones;
        public byte SafetyCarStatus;
        public byte NetworkGame;
        public byte NumWeatherForecastSamples;
        public WeatherForecastSampleBuffer WeatherForecastSamples;
        public byte ForecastAccuracy;
        public byte AiDifficulty;
        public uint SeasonLinkIdentifier;
        public uint WeekendLinkIdentifier;
        public uint SessionLinkIdentifier;
        public byte PitStopWindowIdealLap;
        public byte PitStopWindowLatestLap;
        public byte PitStopRejoinPosition;
        public byte SteeringAssist;
        public byte BrakingAssist;
        public byte GearboxAssist;
        public byte PitAssist;
        public byte PitReleaseAssist;
        public byte ErsAssist;
        public byte DrsAssist;
        public byte DynamicRacingLine;
        public byte DynamicRacingLineType;
        public byte GameMode;
        public byte RuleSet;
        public uint TimeOfDay;
        public byte SessionLength;
        public byte SpeedUnitsLeadPlayer;
        public byte TemperatureUnitsLeadPlayer;
        public byte SpeedUnitsSecondaryPlayer;
        public byte TemperatureUnitsSecondaryPlayer;
        public byte NumSafetyCarPeriods;
        public byte NumVirtualSafetyCarPeriods;
        public byte NumRedFlagPeriods;
        public byte EqualCarPerformance;
        public byte RecoveryMode;
        public byte FlashbackLimit;
        public byte SurfaceType;
        public byte LowFuelMode;
        public byte RaceStarts;
        public byte TyreTemperature;
        public byte PitLaneTyreSim;
        public byte CarDamage;
        public byte CarDamageRate;
        public byte Collisions;
        public byte CollisionsOffForFirstLapOnly;
        public byte MpUnsafePitRelease;
        public byte MpOffForGriefing;
        public byte CornerCuttingStringency;
        public byte ParcFermeRules;
        public byte PitStopExperience;
        public byte SafetyCar;
        public byte SafetyCarExperience;
        public byte FormationLap;
        public byte FormationLapExperience;
        public byte RedFlags;
        public byte AffectsLicenceLevelSolo;
        public byte AffectsLicenceLevelMp;
        public byte NumSessionsInWeekend;
        public WeekendStructureBuffer WeekendStructure;
        public float Sector2LapDistanceStart;
        public float Sector3LapDistanceStart;
    }
}