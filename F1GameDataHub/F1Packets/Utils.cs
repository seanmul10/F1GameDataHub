using F1Packets.F125;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace F1Packets
{
    public static class Utils
    {
        public static T ReadFromBytes<T>(byte[] bytes) where T : struct
        {
            return MemoryMarshal.Read<T>(bytes.AsSpan(0, Marshal.SizeOf<T>()));
        }

        public static string GetNameFromBuffer(NameBuffer nameBuffer)
        {
            var name = new StringBuilder();
            foreach (var c in nameBuffer)
            {
                if (c == 0) break; // Stop at null character
                name.Append((char)c);
            }
            return name.ToString();
        }
    }
}
