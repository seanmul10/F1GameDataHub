using F1.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Packets.F125
{
    public interface IF1Packet
    {
        /// <summary>
        /// The header of the packet containing metadata of the packet and game session.
        /// </summary>
        PacketHeader Header { get; }
    }
}