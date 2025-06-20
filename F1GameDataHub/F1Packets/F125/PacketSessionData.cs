using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class SessionPacketConstants
    {
        public const int MaxMarshalZones = 21;
        public const int MaxWeatherForecastSamples = 64;
        public const int MaxSessionsInWeekend = 12;
    }

    public enum Weather
    {
        Clear = 0,
        LightCloud = 1,
        Overcast = 2,
        LightRain = 3,
        HeavyRain = 4,
        Storm = 5
    }

    public enum SessionType
    {
        Unknown = 0,
        Practice1 = 1,
        Practice2 = 2,
        Practice3 = 3,
        ShortPractice = 4,
        Qualifying1 = 5,
        Qualifying2 = 6,
        Qualifying3 = 7,
        ShortQualifying = 8,
        OneShotQualifying = 9,
        SprintQualifying1 = 10,
        SprintQualifying2 = 11,
        SprintQualifying3 = 12,
        ShortSprintQualifying = 13,
        OneShotSprintQualifying = 14,
        Race = 15,
        Race2 = 16,
        Race3 = 17,
        TimeTrial = 18,
    }

    public enum Formula
    {
        F1Modern = 0,
        F1Classic = 1,
        F2 = 2,
        F1Generic = 3,
        Beta = 4,
        Esports = 6,
        F1World = 8,
        F1Elimination = 9
    }

    public enum SafetyCarStatus
    {
        None = 0,
        Full = 1,
        Virtual = 2,
        FormationLap = 3,
    }

    public enum SessionLength
    {
        None = 0,
        VeryShort = 2,
        Short = 3,
        Medium = 4,
        MediumLong = 5,
        Long = 6,
        Full = 7,
    }

    public enum FlagStatus
    {
        Invalid = -1,
        None = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct MarshalZone
    {
        private readonly float _zoneStart;
        private readonly sbyte _zoneFlag;

        /// <summary>
        /// The fraction of the distance from the start of the track to the marshal zone in meters.
        /// </summary>
        public float ZoneStart => _zoneStart;

        /// <summary>
        /// The flag status of the marshal zone.
        /// </summary>
        public FlagStatus ZoneFlag => (FlagStatus)_zoneFlag;
    }

    [InlineArray(SessionPacketConstants.MaxMarshalZones)]
    public struct MarshalZoneBuffer
    {
        private MarshalZone _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct WeatherForecastSample
    {
        private readonly byte _sessionType;
        private readonly byte _timeOffset;
        private readonly byte _weather;
        private readonly sbyte _trackTemperature;
        private readonly sbyte _trackTemperatureChange;
        private readonly sbyte _airTemperature;
        private readonly sbyte _airTemperatureChange;
        private readonly byte _rainPercentage;

        /// <summary>
        /// The session type for which this weather forecast sample applies.
        /// </summary>
        public SessionType SessionType => (SessionType)_sessionType;

        /// <summary>
        /// The time offset in minutes the forecast sample is expected to arrive.
        /// </summary>
        public byte TimeOffset => _timeOffset;

        /// <summary>
        /// The weather conditions for this forecast sample.
        /// </summary>
        public Weather Weather => (Weather)_weather;

        /// <summary>
        /// The track temperature in degrees Celsius.
        /// </summary>
        public sbyte TrackTemperature => _trackTemperature;

        /// <summary>
        /// The direction of the track temperature change. 0 = up, 1 = down, 2 = no change.
        /// </summary>
        public sbyte TrackTemperatureChange => _trackTemperatureChange;

        /// <summary>
        /// The air temperature in degrees Celsius.
        /// </summary>
        public sbyte AirTemperature => _airTemperature;

        /// <summary>
        /// The direction of the air temperature change. 0 = up, 1 = down, 2 = no change.
        /// </summary>
        public sbyte AirTemperatureChange => _airTemperatureChange;

        /// <summary>
        /// The percentage of rain intensity.
        /// </summary>
        public byte RainPercentage => _rainPercentage;
    }

    [InlineArray(SessionPacketConstants.MaxWeatherForecastSamples)]
    public struct WeatherForecastSampleBuffer
    {
        private WeatherForecastSample _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct WeekendStructure
    {
        private readonly byte _sessionType;

        /// The session type for a given session. See <see cref="F125.SessionType"/> for possible values."/>
        public SessionType Value => (SessionType)_sessionType;
    }

    [InlineArray(SessionPacketConstants.MaxSessionsInWeekend)]
    public struct WeekendStructureBuffer
    {
        private WeekendStructure _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct AssistSettings
    {
        private readonly byte _steeringAssist;
        private readonly byte _brakingAssist;
        private readonly byte _gearboxAssist;
        private readonly byte _pitAssist;
        private readonly byte _pitReleaseAssist;
        private readonly byte _ersAssist;
        private readonly byte _drsAssist;
        private readonly byte _dynamicRacingLine;
        private readonly byte _dynamicRacingLineType;

        /// <summary>
        /// Whether steering assist is enabled.
        /// </summary>
        public bool SteeringAssist => _steeringAssist != 0;

        /// <summary>
        /// The braking assist level. 0 = off, 1 = low, 2 = medium, 3 = high.
        /// </summary>
        public byte BrakingAssist => _brakingAssist;

        /// <summary>
        /// The gearbox assist level. 1 = manual, 2 = manual & suggested gear, 3 = automatic.
        /// </summary>
        public byte GearboxAssist => _gearboxAssist;

        /// <summary>
        /// Whether pit assist is enabled.
        /// </summary>
        public bool PitAssist => _pitAssist != 0;

        /// <summary>
        /// Whether pit release assist is enabled.
        /// </summary>
        public bool PitReleaseAssist => _pitReleaseAssist != 0;

        /// <summary>
        /// Whether ERS assist is enabled.
        /// </summary>
        public bool ErsAssist => _ersAssist != 0;

        /// <summary>
        /// Whether DRS assist is enabled.
        /// </summary>
        public bool DrsAssist => _drsAssist != 0;

        /// <summary>
        /// The level of dynamic racing line assist. 0 = off, 1 = corners only, 2 = full.
        /// </summary>
        public byte DynamicRacingLine => _dynamicRacingLine;

        /// <summary>
        /// Whether 3D dynamic racing line is enabled.
        /// </summary>
        public bool DynamicRacingLineType => _dynamicRacingLineType != 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct SessionSettings
    {
        private readonly byte _equalCarPerformance;
        private readonly byte _recoveryMode;
        private readonly byte _flashbackLimit;
        private readonly byte _surfaceType;
        private readonly byte _lowFuelMode;
        private readonly byte _raceStarts;
        private readonly byte _tyreTemperature;
        private readonly byte _pitLaneTyreSim;
        private readonly byte _carDamage;
        private readonly byte _carDamageRate;
        private readonly byte _collisions;
        private readonly byte _collisionsOffForFirstLapOnly;
        private readonly byte _mpUnsafePitRelease;
        private readonly byte _mpOffForGriefing;
        private readonly byte _cornerCuttingStringency;
        private readonly byte _parcFermeRules;
        private readonly byte _pitStopExperience;
        private readonly byte _safetyCar;
        private readonly byte _safetyCarExperience;
        private readonly byte _formationLap;
        private readonly byte _formationLapExperience;
        private readonly byte _redFlags;
        private readonly byte _affectsLicenceLevelSolo;
        private readonly byte _affectsLicenceLevelMp;

        /// <summary>
        /// Whether the equal car performance setting is enabled.
        /// </summary>
        public bool EqualCarPerformance => _equalCarPerformance != 0;

        /// <summary>
        /// The recovery mode setting for the session. 0 = None, 1 = Flashbacks, 2 = Auto-Recovery.
        /// </summary>
        public byte RecoveryMode => _recoveryMode;

        /// <summary>
        /// The flashback limit setting for the session. 0 = Low, 1 = Medium, 2 = High, 3 = Unlimited.
        /// </summary>
        public byte FlashbackLimit => _flashbackLimit;

        /// <summary>
        /// Whether realistic surface type is enabled. If off, the surface type is simplified.
        /// </summary>
        public bool SurfaceType => _surfaceType != 0;

        /// <summary>
        /// Whether difficult low fuel mode is enabled. If off, the car will not slow down to a crawl when low on fuel.
        /// </summary>
        public bool LowFuelMode => _lowFuelMode != 0;

        /// <summary>
        /// Whether assisted race starts are enabled. If off, race starts will be manual.
        /// </summary>
        public bool RaceStarts => _raceStarts != 0;

        /// <summary>
        /// Whether the full tyre surface and carcass temperature simulation is enabled.
        /// If off, the simulation is limited to tyre surface temperature only.
        /// </summary>
        public bool TyreTemperature => _tyreTemperature != 0;

        /// <summary>
        /// Whether the pit lane tyre simulation is disabled.
        /// If off, the pit lane tyre simulation is enabled.
        /// </summary>
        public bool PitLaneTyreSim => _pitLaneTyreSim != 0; // ToDo: Confusing naming, potentially look at the boolean settings and if they should have better names. 

        /// <summary>
        /// The car damage setting for the session. 0 = Off, 1 = Reduced, 2 = Standard, 3 = Simulation.
        /// </summary>
        public byte CarDamage => _carDamage;

        /// <summary>
        /// The car damage rate setting for the session. 0 = Reduced, 1 = Standard, 2 = Simulation.
        /// </summary>
        public byte CarDamageRate => _carDamageRate;

        /// <summary>
        /// The collisions setting for the session. 0 = Off, 1 = Player-to-Player, 2 = On
        /// </summary>
        public byte Collisions => _collisions;

        /// <summary>
        /// Whether collisions are disabled for the first lap only.
        /// </summary>
        public bool CollisionsOffForFirstLapOnly => _collisionsOffForFirstLapOnly != 0;

        /// <summary>
        /// Whether unsafe pit releases are allowed in multiplayer sessions.
        /// </summary>
        public bool MpUnsafePitRelease => _mpUnsafePitRelease != 0;

        /// <summary>
        /// Whether griefing protection is enabled in multiplayer sessions.
        /// </summary>
        public bool MpOffForGriefing => _mpOffForGriefing != 0;

        /// <summary>
        /// Whether strict corner cutting stringency is enforced.
        /// </summary>
        public bool CornerCuttingStringency => _cornerCuttingStringency != 0;

        /// <summary>
        /// Whether parc ferme rules apply in this session.
        /// </summary>
        public bool ParcFermeRules => _parcFermeRules != 0;

        /// <summary>
        /// The pit stop experience setting. 0 = Automatic, 1 = Broadcast, 2 = Immersive.
        /// </summary>
        public byte PitStopExperience => _pitStopExperience;

        /// <summary>
        /// The safety car frequency setting. 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased.
        /// </summary>
        public byte SafetyCar => _safetyCar;

        /// <summary>
        /// The safety car experience setting. 0 = Broadcast, 1 = Immersive.
        /// </summary>
        public byte SafetyCarExperience => _safetyCarExperience;

        /// <summary>
        /// Whether the formation lap is enabled in this session.
        /// </summary>
        public bool FormationLap => _formationLap != 0;

        /// <summary>
        /// The formation lap experience setting. 0 = Broadcast, 1 = Immersive.
        /// </summary>
        public byte FormationLapExperience => _formationLapExperience;

        /// <summary>
        /// The red flag frequency setting. 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased.
        /// </summary>
        public byte RedFlags => _redFlags;

        /// <summary>
        /// Whether the session affects the players solo licence level.
        /// </summary>
        public bool AffectsLicenceLevelSolo => _affectsLicenceLevelSolo != 0;

        /// <summary>
        /// Whether the session affects the players multiplayer licence level.
        /// </summary>
        public bool AffectsLicenceLevelMp => _affectsLicenceLevelMp != 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketSessionData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _weather;
        private readonly sbyte _trackTemperature;
        private readonly sbyte _airTemperature;
        private readonly byte _totalLaps;
        private readonly ushort _trackLength;
        private readonly byte _sessionType;
        private readonly sbyte _trackId;
        private readonly byte _formula;
        private readonly ushort _sessionTimeLeft;
        private readonly ushort _sessionDuration;
        private readonly byte _pitSpeedLimit;
        private readonly byte _gamePaused;
        private readonly byte _isSpectating;
        private readonly byte _spectatorCarIndex;
        private readonly byte _sliProNativeSupport;
        private readonly byte _numMarshalZones;
        private readonly MarshalZoneBuffer _marshalZones;
        private readonly byte _safetyCarStatus;
        private readonly byte _networkGame;
        private readonly byte _numWeatherForecastSamples;
        private readonly WeatherForecastSampleBuffer _weatherForecastSamples;
        private readonly byte _forecastAccuracy;
        private readonly byte _aiDifficulty;
        private readonly uint _seasonLinkIdentifier;
        private readonly uint _weekendLinkIdentifier;
        private readonly uint _sessionLinkIdentifier;
        private readonly byte _pitStopWindowIdealLap;
        private readonly byte _pitStopWindowLatestLap;
        private readonly byte _pitStopRejoinPosition;
        private readonly AssistSettings _assists;
        private readonly byte _gameMode;
        private readonly byte _ruleSet;
        private readonly uint _timeOfDay;
        private readonly byte _sessionLength;
        private readonly byte _speedUnitsLeadPlayer;
        private readonly byte _temperatureUnitsLeadPlayer;
        private readonly byte _speedUnitsSecondaryPlayer;
        private readonly byte _temperatureUnitsSecondaryPlayer;
        private readonly byte _numSafetyCarPeriods;
        private readonly byte _numVirtualSafetyCarPeriods;
        private readonly byte _numRedFlagPeriods;
        
        private readonly byte _numSessionsInWeekend;
        private readonly WeekendStructureBuffer _weekendStructure;
        private readonly float _sector2LapDistanceStart;
        private readonly float _sector3LapDistanceStart;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The current weather conditions.
        /// </summary>
        public Weather Weather => (Weather)_weather;

        /// <summary>
        /// The track temperature in degrees Celsius.
        /// </summary>
        public sbyte TrackTemperature => _trackTemperature;

        /// <summary>
        /// The air temperature in degrees Celsius.
        /// </summary>
        public sbyte AirTemperature => _airTemperature;

        /// <summary>
        /// The total number of laps in the race.
        /// </summary>
        public byte TotalLaps => _totalLaps;

        /// <summary>
        /// The length of the track in meters.
        /// </summary>
        public ushort TrackLength => _trackLength;

        /// <summary>
        /// The session type. See <see cref="F125.SessionType"/> for possible values.
        /// </summary>
        public SessionType SessionType => (SessionType)_sessionType;

        /// <summary>
        /// The ID of the track, <c>-1 if unknown.</c>
        /// See the documentation for the track ID for each value.
        /// </summary>
        public sbyte TrackId => _trackId;

        /// <summary>
        /// The formula of this session. See <see cref="F125.Formula"/> for possible values.
        /// </summary>
        public Formula Formula => (Formula)_formula;

        /// <summary>
        /// The time left in the session in seconds.
        /// </summary>
        public ushort SessionTimeLeft => _sessionTimeLeft;

        /// <summary>
        /// The total duration of the session in seconds.
        /// </summary>
        public ushort SessionDuration => _sessionDuration;

        /// <summary>
        /// The pit lane speed limit in kilometers per hour.
        /// </summary>
        public byte PitSpeedLimit => _pitSpeedLimit;

        /// <summary>
        /// Whether the game is paused. Only applies to online sessions.
        /// </summary>
        public bool GamePaused => _gamePaused != 0;

        /// <summary>
        /// Whether the player is currently spectating.
        /// </summary>
        public bool IsSpectating => _isSpectating != 0;

        /// <summary>
        /// The index of the car being spectated by the player.
        /// </summary>
        public byte? SpectatorCarIndex => IsSpectating ? _spectatorCarIndex : null;

        /// <summary>
        /// Whether SLI Pro has native support.
        /// </summary>
        public bool SliProNativeSupport => _sliProNativeSupport != 0;

        /// <summary>
        /// The number of marshal zones at the track.
        /// </summary>
        public byte NumMarshalZones => _numMarshalZones;

        /// <summary>
        /// The track status of each marshal zone. See <see cref="NumMarshalZones"/> for the number of marshal zones.
        /// </summary>
        public MarshalZoneBuffer MarshalZones => _marshalZones;

        /// <summary>
        /// The current safety car or formation lap status. See <see cref="F125.SafetyCarStatus"/> for possible values.
        /// </summary>
        public SafetyCarStatus SafetyCarStatus => (SafetyCarStatus)_safetyCarStatus;

        /// <summary>
        /// Whether the current session is online.
        /// </summary>
        public bool NetworkGame => _networkGame != 0;

        /// <summary>
        /// The number of weather forecast samples available.
        /// </summary>
        public byte NumWeatherForecastSamples => _numWeatherForecastSamples;

        /// <summary>
        /// The weather forecast samples for the session. <see cref="NumWeatherForecastSamples"/> for the number of available samples.
        /// </summary>
        public WeatherForecastSampleBuffer WeatherForecastSamples => _weatherForecastSamples;

        /// <summary>
        /// The forecast accuracy setting. True if approximate, false if perfect.
        /// </summary>
        public bool ForecastAccuracy => _forecastAccuracy != 0;

        /// <summary>
        /// The AI difficult level for the session, ranging from 0 to 110.
        /// </summary>
        public byte AiDifficulty => _aiDifficulty;

        /// <summary>
        /// The link identifier for the season. Persists across saves.
        /// </summary>
        public uint SeasonLinkIdentifier => _seasonLinkIdentifier;

        /// <summary>
        /// The link identifier for the race weekend. Persists across saves.
        /// </summary>
        public uint WeekendLinkIdentifier => _weekendLinkIdentifier;

        /// <summary>
        /// The link identifier for the session. Persists across saves.
        /// </summary>
        public uint SessionLinkIdentifier => _sessionLinkIdentifier;

        /// <summary>
        /// The ideal lap for the player to make the next pit stop, according to the game.
        /// </summary>
        public byte PitStopWindowIdealLap => _pitStopWindowIdealLap;

        /// <summary>
        /// The latest lap for the player to make the next pit stop, according to the game.
        /// </summary>
        public byte PitStopWindowLatestLap => _pitStopWindowLatestLap;

        /// <summary>
        /// The predicted position to rejoin the track after a pit stop if the player was to pit now.
        /// </summary>
        public byte PitStopRejoinPosition => _pitStopRejoinPosition;

        /// <summary>
        /// The assist settings allowed in the session.
        /// Individual drivers can use assist levels below or equal to these settings.
        /// </summary>
        public AssistSettings Assists => _assists;

        /// <summary>
        /// The game mode currently being played.
        /// See the documentation for the game mode for each value.
        /// </summary>
        public byte GameMode => _gameMode;

        /// <summary>
        /// The rule set being used in the session.
        /// See the documentation for the rule set for each value.
        /// </summary>
        public byte RuleSet => _ruleSet;

        /// <summary>
        /// The local time of day at the track in minutes since midnight.
        /// </summary>
        public uint TimeOfDay => _timeOfDay;

        /// <summary>
        /// The length of the session. See <see cref="F125.SessionLength"/> for possible values.
        /// </summary>
        public SessionLength SessionLength => (SessionLength)_sessionLength;

        /// <summary>
        /// The speed units being used by the primary player. 0 = MPH, 1 = KPH.
        /// </summary>
        public bool SpeedUnitsLeadPlayer => _speedUnitsLeadPlayer != 0;

        /// <summary>
        /// The temperature units being used by the primary player. 0 = Celcius, 1 = Fahrenheit.
        /// </summary>
        public bool TemperatureUnitsLeadPlayer => _temperatureUnitsLeadPlayer != 0;

        /// <summary>
        /// The speed units being used by the secondary player. 0 = MPH, 1 = KPH.
        /// </summary>
        public bool SpeedUnitsSecondaryPlayer => _speedUnitsSecondaryPlayer != 0;

        /// <summary>
        /// The temperature units being used by the secondary player. 0 = Celcius, 1 = Fahrenheit.
        /// </summary>
        public bool TemperatureUnitsSecondaryPlayer => _temperatureUnitsSecondaryPlayer != 0;

        /// <summary>
        /// The number of full safety car periods there have been in the session.
        /// </summary>
        public byte NumSafetyCarPeriods => _numSafetyCarPeriods;

        /// <summary>
        /// The number of virtual safety car periods there have been in the session.
        /// </summary>
        public byte NumVirtualSafetyCarPeriods => _numVirtualSafetyCarPeriods;

        /// <summary>
        /// The number of red flags there have been in the session.
        /// </summary>
        public byte NumRedFlagPeriods => _numRedFlagPeriods;

        /// <summary>
        /// The number of individual sessions in the weekend.
        /// </summary>
        public byte NumSessionsInWeekend => _numSessionsInWeekend;

        /// <summary>
        /// The list of all session types in the weekend. See <see cref="NumSessionsInWeekend"/> for the number of sessions.
        /// </summary>
        public WeekendStructureBuffer WeekendStructure => _weekendStructure;

        /// <summary>
        /// The distance from the start of the sector 2 to the end of the lap in meters.
        /// </summary>
        public float Sector2LapDistanceStart => _sector2LapDistanceStart;

        /// <summary>
        /// The distance from the start of the sector 3 to the end of the lap in meters.
        /// </summary>
        public float Sector3LapDistanceStart => _sector3LapDistanceStart;
    }
}