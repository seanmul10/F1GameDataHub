using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public enum PitStatus : byte
    {
        None = 0,
        Pitting = 1,
        InPitArea = 2
    }

    public enum Sector : byte
    {
        Sector1 = 0,
        Sector2 = 1,
        Sector3 = 2
    }

    public enum DriverStatus : byte
    {
        InGarage = 0,
        FlyingLap = 1,
        InLap = 2,
        OutLap = 3,
        OnTrack = 4,
    }

    public enum ResultStatus : byte
    {
        Invalid = 0,
        Inactive = 1,
        Active = 2,
        Finished = 3,
        DNF = 4,
        DSQ = 5,
        NotClassified = 6,
        Retired = 7
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct LapData
    {
        private readonly uint _lastLapTimeInMS;
        private readonly uint _currentLapTimeInMS;
        private readonly ushort _sector1TimeMSPart;
        private readonly byte _sector1TimeMinutesPart;
        private readonly ushort _sector2TimeMSPart;
        private readonly byte _sector2TimeMinutesPart;
        private readonly ushort _deltaToCarInFrontMSPart;
        private readonly byte _deltaToCarInFrontMinutesPart;
        private readonly ushort _deltaToRaceLeaderMSPart;
        private readonly byte _deltaToRaceLeaderMinutesPart;
        private readonly float _lapDistance;
        private readonly float _totalDistance;
        private readonly float _safetyCarDelta;
        private readonly byte _carPosition;
        private readonly byte _currentLapNum;
        private readonly byte _pitStatus;
        private readonly byte _numPitStops;
        private readonly byte _sector;
        private readonly byte _currentLapInvalid;
        private readonly byte _penalties;
        private readonly byte _totalWarnings;
        private readonly byte _cornerCuttingWarnings;
        private readonly byte _numUnservedDriveThroughPens;
        private readonly byte _numUnservedStopGoPens;
        private readonly byte _gridPosition;
        private readonly byte _driverStatus;
        private readonly byte _resultStatus;
        private readonly byte _pitLaneTimerActive;
        private readonly ushort _pitLaneTimeInLaneInMS;
        private readonly ushort _pitStopTimerInMS;
        private readonly byte _pitStopShouldServePen;
        private readonly float _speedTrapFastestSpeed;
        private readonly byte _speedTrapFastestLap;

        /// <summary>
        /// The previous lap time in milliseconds.
        /// </summary>
        public uint LastLapTimeInMS => _lastLapTimeInMS;

        /// <summary>
        /// The current lap time in milliseconds.
        /// </summary>
        public uint CurrentLapTimeInMS => _currentLapTimeInMS;

        /// <summary>
        /// The current sector 1 time in milliseconds.
        /// </summary>
        public uint Sector1TimeInMS => (uint)(_sector1TimeMSPart + (_sector1TimeMinutesPart * 60000));

        /// <summary>
        /// The current sector 2 time in milliseconds.
        /// </summary>
        public uint Sector2TimeInMS => (uint)(_sector2TimeMSPart + (_sector2TimeMinutesPart * 60000));

        /// <summary>
        /// The current sector 3 time in milliseconds.
        /// </summary>
        public uint Sector3TimeInMS => CurrentLapTimeInMS - Sector1TimeInMS - Sector2TimeInMS;

        /// <summary>
        /// The delta time to the car in front in milliseconds.
        /// </summary>
        public uint DeltaToCarInFrontInMS => (uint)(_deltaToCarInFrontMSPart + (_deltaToCarInFrontMinutesPart * 60000));

        /// <summary>
        /// The delta time to the current leader of the race in milliseconds.
        /// </summary>
        public uint DeltaToRaceLeaderInMS => (uint)(_deltaToRaceLeaderMSPart + (_deltaToRaceLeaderMinutesPart * 60000));

        /// <summary>
        /// The distance covered in the current lap in meters.
        /// </summary>
        public float LapDistance => _lapDistance;

        /// <summary>
        /// The total distance covered in the race so far in meters.
        /// </summary>
        public float TotalDistance => _totalDistance;

        /// <summary>
        /// The delta to the maximum allowed time in milliseconds during a safety car period.
        /// </summary>
        public float SafetyCarDelta => _safetyCarDelta;

        /// <summary>
        /// The position of the car in the race currently.
        /// </summary>
        public byte CarPosition => _carPosition;

        /// <summary>
        /// The lap number the car is currently on.
        /// </summary>
        public byte CurrentLapNum => _currentLapNum;

        /// <summary>
        /// The pit status of the car. See <see cref="PitStatus"/> for possible values.
        /// </summary>
        public PitStatus PitStatus => (PitStatus)_pitStatus;

        /// <summary>
        /// The number of pit stops the car has made in the race so far.
        /// </summary>
        public byte NumPitStops => _numPitStops;

        /// <summary>
        /// The sector of the track the car is currently in. See <see cref="Sector"/> for possible values.
        /// </summary>
        public Sector Sector => (Sector)_sector;

        /// <summary>
        /// Whether the current lap is invalid.
        /// </summary>
        public bool CurrentLapInvalid => _currentLapInvalid != 0;

        /// <summary>
        /// The accumulated time penalty for the car in seconds.
        /// </summary>
        public byte Penalties => _penalties;

        /// <summary>
        /// The total accumulated number of warnings for the car.
        /// </summary>
        public byte TotalWarnings => _totalWarnings;

        /// <summary>
        /// The accumulated number of corner cutting warnings for the car.
        /// </summary>
        public byte CornerCuttingWarnings => _cornerCuttingWarnings;

        /// <summary>
        /// The number of unserved drive-through penalties for the car.
        /// </summary>
        public byte NumUnservedDriveThroughPens => _numUnservedDriveThroughPens;

        /// <summary>
        /// The number of unserved stop-go penalties for the car.
        /// </summary>
        public byte NumUnservedStopGoPens => _numUnservedStopGoPens;

        /// <summary>
        /// The grid position the car started the race in.
        /// </summary>
        public byte GridPosition => _gridPosition;

        /// <summary>
        /// The current status of the driver. See <see cref="F125.DriverStatus"/> for possible values.
        /// </summary>
        public Sector DriverStatus => (Sector)_driverStatus;

        /// <summary>
        /// The result status of the car. See <see cref="F125.ResultStatus"/> for possible values.
        /// </summary>
        public ResultStatus ResultStatus => (ResultStatus)_resultStatus;

        /// <summary>
        /// Whether the pit lane timer is active currently.
        /// </summary>
        public bool PitLaneTimerActive => _pitLaneTimerActive != 0;

        /// <summary>
        /// The time currently being spent in the pit lane in milliseconds. Only valid if the pit lane timer is active.
        /// </summary>
        public ushort PitLaneTimeInLaneInMS => _pitLaneTimeInLaneInMS;

        /// <summary>
        /// The time currently being spent in the pit box in milliseconds. Only valid if the pit lane timer is active.
        /// </summary>
        public ushort PitStopTimerInMS => _pitStopTimerInMS;

        /// <summary>
        /// Whether the car should serve a penalty during the next pit stop.
        /// </summary>
        public bool PitStopShouldServePen => _pitStopShouldServePen != 0;

        /// <summary>
        /// The fastest speed through the speed trap recorded for this car in kilometers per hour in this session.
        /// </summary>
        public float SpeedTrapFastestSpeed => _speedTrapFastestSpeed;

        /// <summary>
        /// The number of the fastest lap recorded through the speed trap for this car. <c>null</c> if speed trap has not been passed by this car in the session.
        /// </summary>
        public byte? SpeedTrapFastestLap =>
            _speedTrapFastestLap == 255 ? null : _speedTrapFastestLap;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct LapDataBuffer
    {
        private LapData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLapData : IF1Packet
    {
        private readonly PacketHeader _header;
        private LapDataBuffer _lapData;
        private readonly byte _timeTrialPBCarIdx;
        private readonly byte _timeTrialRivalCarIdx;

        /// <inheritdoc />
        public readonly PacketHeader Header => _header;

        /// <summary>
        /// The lap data for each car in the session.
        /// </summary>
        public readonly LapDataBuffer LapData => _lapData;

        /// <summary>
        /// The car index that holds the personal best time. Only valid in Time Trial sessions.
        /// </summary>
        public readonly byte TimeTrialPBCarIdx => _timeTrialPBCarIdx;

        /// <summary>
        /// The car index that of the selected rival. Only valid in Time Trial sessions.
        /// </summary>
        public readonly byte TimeTrialRivalCarIdx => _timeTrialRivalCarIdx;
    }
}