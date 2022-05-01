using ProtoMC.Network.DataTypes;
using ProtoMC.Network.IO;
using ProtoMC.Network.Packets;
using System.Text;

namespace ProtoMC.Utils
{
    public static class Extensions
    {
        public static byte[] GetDispose(this MemoryStream stream)
        {
            byte[] b = stream.ToArray();
            stream.Dispose();
            return b;
        }

        public static string Decode(this byte[] b) => Encoding.UTF8.GetString(b);
        public static byte[] Encode(this string s) => Encoding.UTF8.GetBytes(s);
    }
}
