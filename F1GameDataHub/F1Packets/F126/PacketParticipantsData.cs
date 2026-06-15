using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace F1Packets.F126
{
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

        public byte Red => _red;
        public byte Green => _green;
        public byte Blue => _blue;
        public string HexColour => $"#{_red:X2}{_green:X2}{_blue:X2}";
    }

    [InlineArray(4)]
    public struct LiveryColourBuffer
    {
        private LiveryColour _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ParticipantData
    {
        private readonly byte _aiControlled;
        private readonly ushort _driverId;
        private readonly ushort _networkId;
        private readonly ushort _teamId;
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

        public bool AiControlled => _aiControlled != 0;
        public ushort? DriverId => _driverId == ushort.MaxValue ? null : _driverId;
        public ushort NetworkId => _networkId;
        public ushort TeamId => _teamId;
        public bool MyTeam => _myTeam != 0;
        public byte RaceNumber => _raceNumber;
        public byte Nationality => _nationality;
        public string? Name => PacketText.GetNameFromBuffer(_name);
        public bool YourTelemetry => _yourTelemetry != 0;
        public bool ShowOnlineNames => _showOnlineNames != 0;
        public ushort TechLevel => _techLevel;
        public Platform? Platform => _platform == 255 ? null : (Platform)_platform;
        public byte NumColours => _numColours;
        public LiveryColourBuffer LiveryColours => _liveryColours;
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

        public PacketHeader Header => _header;
        public byte NumActiveCars => _numActiveCars;
        public ParticipantDataBuffer Participants => _participants;
    }

    internal static class PacketText
    {
        public static string? GetNameFromBuffer(NameBuffer nameBuffer)
        {
            var name = new StringBuilder();
            foreach (var c in nameBuffer)
            {
                if (c == 0)
                {
                    break;
                }

                name.Append((char)c);
            }

            return name.Length == 0 ? null : name.ToString();
        }
    }
}
