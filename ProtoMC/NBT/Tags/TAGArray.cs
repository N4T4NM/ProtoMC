using ProtoMC.Network.DataTypes;

namespace ProtoMC.NBT.Tags
{
    public class TAGArray<T> : ITag where T : struct
    {
        public ProtoArray<long> Value { get; set; } = new(new long[0], true);

        public async Task ReadAsync(Stream stream) => await Value.DeserializeAsync(stream);
        public async Task WriteAsync(Stream stream) => await Value.SerializeAsync(stream);
    }
}
