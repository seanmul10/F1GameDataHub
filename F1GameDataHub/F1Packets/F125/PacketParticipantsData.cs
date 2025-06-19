using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace F1Packets.F125
{
    public static class ParticipantsPacketConstants
    {
        public const int LiveryColours = 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LiveryColour
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    [InlineArray(ParticipantsPacketConstants.LiveryColours)]
    public struct LiveyColourBuffer
    {
        private LiveryColour _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticipantData
    {
        public byte AiControlled;
        public byte DriverId;
        public byte NetworkId;
        public byte TeamId;
        public byte MyTeam;
        public byte RaceNumber;
        public byte Nationality;
        private NameBuffer _name;
        public byte YourTelemetry;
        public byte ShowOnlineNames;
        public ushort TechLevel;
        public byte Platform;
        public byte NumColours;
        public LiveyColourBuffer LiveryColours;

        public readonly string Name => Utils.GetNameFromBuffer(_name);
    }

    [InlineArray(PacketConstants.MaxNumCarsInUdpData)]
    public struct ParticipantDataBuffer
    {
        private ParticipantData _element0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketParticipantsData
    {
        public PacketHeader Header;
        public byte NumActiveCars;
        public ParticipantDataBuffer Participants;
    }
}