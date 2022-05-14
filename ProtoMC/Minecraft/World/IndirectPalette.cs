using ProtoMC.Network.DataTypes;
using ProtoMC.Utils;

namespace ProtoMC.Minecraft.World
{
    public class IndirectPalette : IPalette
    {
        public byte BitsPerBlock { get; set; }
        public int MaxBits { get; set; }
        public ProtoArray<VarInt> Palette { get; set; }
        public BitArray Data { get; set; }

        public IndirectPalette(byte bitsPerBlock, int capacity, int maxBits, ProtoArray<VarInt> palette)
        {
            BitsPerBlock = bitsPerBlock;
            MaxBits = maxBits;
            Palette = palette;

            Data = new(bitsPerBlock, capacity);
        }

        public async Task DeserializeAsync(Stream stream) => await Data.ReadStreamAsync(stream);
        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(BitsPerBlock);
            await Palette.SerializeAsync(stream);
            await new VarInt() { Value = Data.BufferSize }.SerializeAsync(stream);
            await Data.WriteToStreamAsync(stream);
        }
    }
}
