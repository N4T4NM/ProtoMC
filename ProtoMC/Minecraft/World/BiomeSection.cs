using ProtoMC.Network.DataTypes;
using ProtoMC.Utils;

namespace ProtoMC.Minecraft.World
{
    public class BiomeSection
    {
        public byte BitsPerBlock { get; set; }
        public IPalette Palette { get; set; } = new SingleValuePalette(0);

        public const int MAX_BITS_PER_BIOME = 3;
        public const int BIOME_SECTION_VOLUME = ChunkSection.BLOCK_SECTION_VOLUME / (4 * 4 * 4) | 0;

        public async Task DeserializeAsync(Stream stream)
        {
            BitsPerBlock = await stream.ReadStructAsync<byte>();

            if (BitsPerBlock == 0)
            {
                SingleValuePalette svp = new(0);
                await svp.DeserializeAsync(stream);
                await stream.ReadStructAsync<byte>();

                Palette = svp;
                return;
            }

            if (BitsPerBlock > MAX_BITS_PER_BIOME)
            {
                await new VarInt().DeserializeAsync(stream);
                DirectPalette dp = new(BitsPerBlock, BIOME_SECTION_VOLUME);
                await dp.DeserializeAsync(stream);

                Palette = dp;
                return;
            }

            ProtoArray<VarInt> palette = new(new VarInt[0]);
            await palette.DeserializeAsync(stream);

            await new VarInt().DeserializeAsync(stream);

            IndirectPalette ip = new(BitsPerBlock, BIOME_SECTION_VOLUME, MAX_BITS_PER_BIOME, palette);
            await ip.DeserializeAsync(stream);

            Palette = ip;
        }
        public async Task SerializeAsync(Stream stream) => await Palette.SerializeAsync(stream);
    }
}
