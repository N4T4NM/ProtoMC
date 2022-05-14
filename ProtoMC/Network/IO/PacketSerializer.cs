using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets;
using ProtoMC.Utils;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ProtoMC.Network.IO
{
    public static class PacketSerializer
    {
        public static void ChangeEndianess(byte[] buffer)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
        }

        public static async Task SerializeDataTypeAsync(IDataType dataType, Stream stream)
            => await dataType.SerializeAsync(stream);
        public static async Task SerializeStructAsync(object structure, Stream stream)
        {
            if (structure is bool)
            {
                stream.WriteByte((byte)(((bool)structure) ? 0x1 : 0x0));
                return;
            }

            int sz = Marshal.SizeOf(structure);
            IntPtr bufferPtr = Marshal.AllocHGlobal(sz);
            Marshal.StructureToPtr(structure, bufferPtr, false);

            byte[] buffer = new byte[sz];
            Marshal.Copy(bufferPtr, buffer, 0, sz);
            Marshal.FreeHGlobal(bufferPtr);

            ChangeEndianess(buffer);
            await stream.WriteAsync(buffer);
        }
        public static async Task<byte[]> SerializePacketAsync(IPacket packet)
        {
            Type type = packet.GetType();
            MemoryStream serializationStream = new();

            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = prop.GetValue(packet)!;
                if (prop.PropertyType.IsAssignableTo(typeof(IDataType)))
                    await SerializeDataTypeAsync((IDataType)value, serializationStream);
                else await SerializeStructAsync(value, serializationStream);
            }

            return serializationStream.GetDispose();
        }

        public static async Task<object> TypeToStructureAsync(Type type, Stream stream)
        {
            if(type == typeof(bool))
            {
                byte[] b = new byte[1];
                await stream.ReadAsync(b);

                return b[0] == 0x01;
            }
            int sz = Marshal.SizeOf(type);

            byte[] data = new byte[sz];
            await stream.ReadAsync(data);

            ChangeEndianess(data);

            IntPtr structPtr = Marshal.AllocHGlobal(sz);
            Marshal.Copy(data, 0, structPtr, sz);
            object value = Marshal.PtrToStructure(structPtr, type)!;

            Marshal.FreeHGlobal(structPtr);
            return value;
        }

        public static async Task DeserializeDataTypeAsync(IPacket packet, PropertyInfo property, Stream stream)
        {
            IDataType dType = (IDataType)property.GetValue(packet)!;
            await dType.DeserializeAsync(stream);
        }
        public static async Task DeserializeStructAsync(IPacket packet, PropertyInfo property, Stream stream)
        {
            if (property.PropertyType == typeof(bool))
            {
                byte[] boolByte = new byte[1];
                await stream.ReadAsync(boolByte);

                property.SetValue(packet, boolByte[0] == 0x1);
                return;
            }

            object value = await TypeToStructureAsync(property.PropertyType, stream);

            property.SetValue(packet, value);
        }
        public static async Task DeserializeDataIntoPacketAsync(byte[] data, IPacket packet)
        {
            MemoryStream stream = new(data);
            PropertyInfo[] props = packet.GetType().GetProperties();

            for (int i = 1; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                if (prop.PropertyType.IsAssignableTo(typeof(IDataType)))
                    await DeserializeDataTypeAsync(packet, prop, stream);
                else await DeserializeStructAsync(packet, prop, stream);
            }

            stream.Dispose();
        }
    }
}
