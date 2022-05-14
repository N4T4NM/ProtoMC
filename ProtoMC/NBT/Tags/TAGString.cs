using ProtoMC.Network.IO;
using ProtoMC.Utils;

namespace ProtoMC.NBT.Tags
{
    public class TAGString : ITag
    {
        public string Value { get; set; } = "";
        public async Task ReadAsync(Stream stream)
        {
            ushort len = (ushort)await PacketSerializer.TypeToStructureAsync(typeof(ushort), stream);
            byte[] bytes = new byte[len];
            await stream.ReadAsync(bytes);

            Value = bytes.Decode();
        }

        public async Task WriteAsync(Stream stream)
        {
            ushort len = (ushort)Value.Length;
            await PacketSerializer.SerializeStructAsync(len, stream);

            await stream.WriteAsync(Value.Encode()); //TODO: Check for glitches with Modified UTF-8
        }
    }
}
