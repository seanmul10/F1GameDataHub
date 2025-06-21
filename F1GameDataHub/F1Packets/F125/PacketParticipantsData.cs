using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public static class ParticipantsPacketConstants
    {
        public const int LiveryColours = 4;
    }

    public enum Platform
    {
        Steam = 1,
        PlayStation = 3,
        Xbox = 4,
        Origin = 6,
        Unknown = 255
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct LiveryColour
    {
        private readonly byte _red;
        private readonly byte _green;
        private readonly byte _blue;

        /// <summary>
        /// The red component of the livery colour, from 0 to 255.
        /// </summary>
        public readonly byte Red => _red;

        /// <summary>
        /// The green component of the livery colour, from 0 to 255.
        /// </summary>
        public readonly byte Green => _green;

        /// <summary>
        /// The blue component of the livery colour, from 0 to 255.
        /// </summary>
        public readonly byte Blue => _blue;

        /// <summary>
        /// The colour of the livery in HEX format.
        /// </summary>
        public readonly string HexColour =>
            $"#{_red:X2}{_green:X2}{_blue:X2}";
    }

    [InlineArray(ParticipantsPacketConstants.LiveryColours)]
    public struct LiveryColourBuffer
    {
        private LiveryColour _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ParticipantData
    {
        private readonly byte _aiControlled;
        private readonly byte _driverId;
        private readonly byte _networkId;
        private readonly byte _teamId;
        private readonly byte _myTeam;
        private readonly byte _raceNumber;
        private readonly byte _nationality;
        private readonly NameBuffer _name;
        private readonly byte _yourTelemetry;
        private readonly byte _showOnlineNames;
        private readonly ushort _techLevel;
        private readonly byte _platform;
        private readonly byte _numColours;
        private readonly LiveryColourBuffer _liveryColours;

        /// <summary>
        /// Whether the driver is AI controlled.
        /// </summary>
        public readonly bool AiControlled => _aiControlled != 0;

        /// <summary>
        /// The ID of the real or EA created driver. <c>null</c> if the driver is network human.
        /// See the documentation for the driver name for each ID.
        /// </summary>
        public readonly byte? DriverId =>
            _driverId == 255 ? null : _driverId;

        /// <summary>
        /// The unique identifier for network players in the session.
        /// </summary>
        public readonly byte NetworkId => _networkId;

        /// <summary>
        /// The ID of the team the driver is racing for.
        /// See the documentation for the team name for each ID.
        /// </summary>
        public readonly byte TeamId => _teamId;

        /// <summary>
        /// Whether the car is the player's team car in the My Team mode.
        /// </summary>
        public readonly bool MyTeam => _myTeam != 0;

        /// <summary>
        /// The race number of the driver.
        /// </summary>
        public readonly byte RaceNumber => _raceNumber;

        /// <summary>
        /// The ID of the nationality of the driver.
        /// See the documentation for the nationality for each ID.
        /// </summary>
        public readonly byte Nationality => _nationality;

        /// <summary>
        /// The name of the driver. <c>null</c> if the name of the driver is not available,
        /// or <c>"Player"</c> if the players <see cref="ShowOnlineNames"/> setting is off.
        /// May be truncated with ... if the name is too long.
        /// </summary>
        public readonly string? Name => Utils.GetNameFromBuffer(_name);

        /// <summary>
        /// Whether the player has public telemetry output disabled in their settings.
        /// If restricted their sensitive telemetry data will be limited.
        /// </summary>
        public readonly bool YourTelemetry => _yourTelemetry != 0;

        /// <summary>
        /// Whether the player has online names shown in their telemetry settings.
        /// </summary>
        public readonly bool ShowOnlineNames => _showOnlineNames != 0;

        /// <summary>
        /// The players F1 World tech level.
        /// </summary>
        public readonly ushort TechLevel => _techLevel;

        /// <summary>
        /// The platform this player is playing on.
        /// </summary>
        public readonly Platform? Platform => (Platform)_platform;

        /// <summary>
        /// The number of colours available to use for the livery of this car.
        /// </summary>
        public readonly byte NumColours => _numColours;

        /// <summary>
        /// The livery colours being used on this car.
        /// </summary>
        public readonly LiveryColourBuffer LiveryColours => _liveryColours;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct ParticipantDataBuffer
    {
        private ParticipantData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketParticipantsData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _numActiveCars;
        private readonly ParticipantDataBuffer _participants;

        /// <inheritdoc/>
        public readonly PacketHeader Header => _header;

        /// <summary>
        /// The number of active cars in the session.
        /// </summary>
        public readonly byte NumActiveCars => _numActiveCars;

        /// <summary>
        /// The data for each participant in the session.
        /// </summary>
        public readonly ParticipantDataBuffer Participants => _participants;
    }
}