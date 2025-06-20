using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct LobbyInfoData
    {
        private readonly byte _aiControlled;
        private readonly byte _teamId;
        private readonly byte _nationality;
        private readonly byte _platform;
        private readonly NameBuffer _name;
        private readonly byte _carNumber;
        private readonly byte _yourTelemetry;
        private readonly byte _showOnlineNames;
        private readonly ushort _techLevel;
        private readonly byte _readyStatus;

        /// <summary>
        /// Whether the driver is AI controlled.
        /// </summary>
        public bool AiControlled => _aiControlled != 0;

        /// <summary>
        /// The ID of the team the driver is racing for.
        /// See the documentation for the team name for each ID.
        /// </summary>
        public byte TeamId => _teamId;

        /// <summary>
        /// The ID of the nationality of the driver.
        /// See the documentation for the nationality for each ID.
        /// </summary>
        public byte Nationality => _nationality;

        /// <summary>
        /// The platform this player is playing on.
        /// </summary>
        public Platform? Platform => (Platform)_platform;

        /// <summary>
        /// The name of the driver. <c>null</c> if the name of the driver is not available,
        /// or <c>"Player"</c> if the players <see cref="ShowOnlineNames"/> setting is off.
        /// May be truncated with ... if the name is too long.
        /// </summary>
        public string? Name => Utils.GetNameFromBuffer(_name);

        /// <summary>
        /// The race number of the driver.
        /// </summary>
        public byte RaceNumber => _carNumber;

        /// <summary>
        /// Whether the player has public telemetry output disabled in their settings.
        /// If restricted their sensitive telemetry data will be limited.
        /// </summary>
        public bool YourTelemetry => _yourTelemetry != 0;

        /// <summary>
        /// Whether the player has online names shown in their telemetry settings.
        /// </summary>
        public bool ShowOnlineNames => _showOnlineNames != 0;

        /// <summary>
        /// The players F1 World tech level.
        /// </summary>
        public ushort TechLevel => _techLevel;

        /// <summary>
        /// The ready status of the player. 0 = not ready, 1 = ready, 2 = spectating.
        /// </summary>
        public byte ReadyStatus => _readyStatus;
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct LobbyInfoDataBuffer
    {
        private LobbyInfoData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PacketLobbyInfoData : IF1Packet
    {
        private readonly PacketHeader _header;
        private readonly byte _numPlayers;
        private readonly LobbyInfoDataBuffer _lobbyPlayers;

        /// <inheritdoc/>
        public PacketHeader Header => _header;

        /// <summary>
        /// The number of players in the lobby.
        /// </summary>
        public byte NumPlayers => _numPlayers;

        /// <summary>
        /// The info for each player in the lobby.
        /// </summary>
        public LobbyInfoDataBuffer LobbyPlayers => _lobbyPlayers;
    }
}