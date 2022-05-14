using ProtoMC.Network.IO;
using System.Reflection;
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

        public static object Instantiate(this Type type)
        {
            ConstructorInfo ctor = type.GetConstructors()[0];
            ParameterInfo[] ctorParams = ctor.GetParameters();

            object?[] paramsInfo = new object?[ctorParams.Length];
            for (int i = 0; i < ctorParams.Length; i++)
            {
                ParameterInfo param = ctorParams[i];
                if (param.ParameterType.IsValueType)
                    paramsInfo[i] = Activator.CreateInstance(param.ParameterType);
            }

            return Activator.CreateInstance(type, paramsInfo)!;
        }

        public static async Task<T> ReadStructAsync<T>(this Stream stream)
        {
            object obj = await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
            return (T)obj;
        }
        public static async Task WriteStructAsync<T>(this Stream stream, T obj)
            => await PacketSerializer.SerializeStructAsync(obj, stream);

        public static byte[] GetBytes(this Stream stream, long start, long length)
        {
            byte[] b = new byte[length];
            long pos = stream.Position;

            stream.Position = start;
            stream.Read(b);

            stream.Position = pos;

            return b;
        }

        public static bool Compare(this byte[] a, byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
                if (a[i] != b[i]) return false;

            return true;
        }
    }
}
