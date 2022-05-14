using ProtoMC.Network.DataTypes;
using ProtoMC.Utils;

namespace ProtoMC.Minecraft.World
{
    public class DirectPalette : IPalette
    {
        public byte BitsPerBlock { get; set; }
        public BitArray Data { get; set; }

        public DirectPalette(byte bitsPerBlock, int capacity)
        {
            BitsPerBlock = bitsPerBlock;
            Data = new(bitsPerBlock, capacity);
        }

        public async Task DeserializeAsync(Stream stream) => await Data.ReadStreamAsync(stream);
        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(BitsPerBlock);
            await new VarInt() { Value = Data.BufferSize }.SerializeAsync(stream);
            await Data.WriteToStreamAsync(stream);
        }
    }
}
