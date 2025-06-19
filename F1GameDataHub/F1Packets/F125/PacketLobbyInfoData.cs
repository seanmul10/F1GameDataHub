using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LobbyInfoData
    {
        public byte AiControlled;
        public byte TeamId;
        public byte Nationality;
        public byte Platform;
        private NameBuffer _name;
        public byte CarNumber;
        public byte YourTelemetry;
        public byte ShowOnlineNames;
        public ushort TechLevel;
        public byte ReadyStatus;

        public readonly string Name => Utils.GetNameFromBuffer(_name);
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct LobbyInfoDataBuffer
    {
        private LobbyInfoData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLobbyInfoData
    {
        public PacketHeader Header;
        public byte NumPlayers;
        public LobbyInfoDataBuffer LobbyPlayers;
    }
}