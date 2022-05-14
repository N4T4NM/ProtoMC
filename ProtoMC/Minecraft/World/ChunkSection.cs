using ProtoMC.Network.DataTypes;
using ProtoMC.Utils;

namespace ProtoMC.Minecraft.World
{
    public class ChunkSection
    {
        public short BlockCount { get; set; }
        public byte BitsPerBlock { get; set; }

        public const byte MAX_BITS_PER_BLOCK = 8;
        public const int BLOCK_SECTION_VOLUME = 16 * 16 * 16;

        public IPalette Palette { get; set; } = new SingleValuePalette(0);

        public async Task DeserializeAsync(Stream stream)
        {
            BlockCount = await stream.ReadStructAsync<short>();
            BitsPerBlock = await stream.ReadStructAsync<byte>();

            if (BitsPerBlock == 0)
            {
                SingleValuePalette svp = new(0);
                await svp.DeserializeAsync(stream);
                await stream.ReadStructAsync<byte>();

                Palette = svp;
                return;
            }

            if (BitsPerBlock > MAX_BITS_PER_BLOCK)
            {
                await new VarInt().DeserializeAsync(stream);
                DirectPalette dp = new(BitsPerBlock, BLOCK_SECTION_VOLUME);
                await dp.DeserializeAsync(stream);

                Palette = dp;
                return;
            }

            ProtoArray<VarInt> palette = new(new VarInt[0]);
            await palette.DeserializeAsync(stream);

            await new VarInt().DeserializeAsync(stream);

            IndirectPalette ip = new(BitsPerBlock, BLOCK_SECTION_VOLUME, MAX_BITS_PER_BLOCK, palette);
            await ip.DeserializeAsync(stream);

            Palette = ip;
        }

        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(BlockCount);
            await Palette.SerializeAsync(stream);
        }
    }
}
