using ProtoMC.Network.IO;

namespace ProtoMC.NBT.Tags
{
    public class TAGShort : ITag
    {
        public short Value { get; set; }

        public async Task ReadAsync(Stream stream)
            => Value = (short)await PacketSerializer.TypeToStructureAsync(typeof(short), stream);
        public async Task WriteAsync(Stream stream)
            => await PacketSerializer.SerializeStructAsync(Value, stream);
    }
}
