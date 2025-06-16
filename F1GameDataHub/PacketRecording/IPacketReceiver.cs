using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PacketRecording
{
    public interface IPacketReceiver
    {
        Task<UdpReceiveResult> ReceiveAsync(CancellationToken token);
    }
}
