using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct TimeTrialDataSet
    {
        private readonly byte _carIdx;
        private readonly byte _teamId;
        private readonly uint _lapTimeInMS;
        private readonly uint _sector1TimeInMS;
        private readonly uint _sector2TimeInMS;
        private readonly uint _sector3TimeInMS;
        private readonly byte _tractionControl;
        private readonly byte _gearboxAssist;
        private readonly byte _antiLockBrakes;
        private readonly byte _equalCarPerformance;
        private readonly byte _customSetup;
        private readonly byte _valid;

        /// <summary>
        /// The index of the car.
        /// </summary>
        public byte CarIdx => _carIdx;

        /// <summary>
        /// The team ID of the car.
        /// </summary>
        public byte TeamId => _teamId;

        /// <summary>
        /// The lap time of the car in milliseconds.
        /// </summary>
        public uint LapTimeInMS => _lapTimeInMS;

        /// <summary>
        /// The sector 1 time of the car in milliseconds.
        /// </summary>
        public uint Sector1TimeInMS => _sector1TimeInMS;

        /// <summary>
        /// The sector 2 time of the car in milliseconds.
        /// </summary>
        public uint Sector2TimeInMS => _sector2TimeInMS;

        /// <summary>
        /// The sector 3 time of the car in milliseconds.
        /// </summary>
        public uint Sector3TimeInMS => _sector3TimeInMS;

        /// <summary>
        /// Whether the car had traction control assist enabled during the lap.
        /// </summary>
        public bool TractionControl => _tractionControl != 0;

        /// <summary>
        /// Whether the car had gearbox assist enabled during the lap.
        /// </summary>
        public bool GearboxAssist => _gearboxAssist != 0;

        /// <summary>
        /// Whether the car had anti-lock brakes assist enabled during the lap.
        /// </summary>
        public bool AntiLockBrakes => _antiLockBrakes != 0;

        /// <summary>
        /// Whether the car performance is set to equal for the lap.
        /// </summary>
        public bool EqualCarPerformance => _equalCarPerformance != 0;

        /// <summary>
        /// Whether the driver used a custom setup to set the lap.
        /// </summary>
        public bool CustomSetup => _customSetup != 0;

        /// <summary>
        /// Whether the lap time is valid.
        /// </summary>
        public bool Valid => _valid != 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketTimeTrialData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly TimeTrialDataSet _playerSessionBestDataSet;
        private readonly TimeTrialDataSet _personalBestDataSet;
        private readonly TimeTrialDataSet _rivalDataSet;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The best lap time of the player in the current session.
        /// </summary>
        public TimeTrialDataSet PlayerSessionBestDataSet => _playerSessionBestDataSet;

        /// <summary>
        /// The personal best lap time of the player across all sessions.
        /// </summary>
        public TimeTrialDataSet PersonalBestDataSet => _personalBestDataSet;

        /// <summary>
        /// The personal best lap time of the selected rival in the current session.
        /// </summary>
        public TimeTrialDataSet RivalDataSet => _rivalDataSet;
    }
}