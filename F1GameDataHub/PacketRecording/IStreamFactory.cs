using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketRecording
{
    public interface IStreamFactory
    {
        Stream CreateStream(string filePath);
    }
}
