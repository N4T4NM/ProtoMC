using ProtoMC.Minecraft.World;
using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class ChunkData : IDataType
    {
        public List<ChunkSection> Sections { get; set; } = new();
        public List<BiomeSection> Biomes { get; set; } = new();

        public const int SECTIONS_NUM = 384 >> 3;

        public async Task ReadSectionsAsync(Stream stream)
        {
            Sections.Clear();
            Biomes.Clear();

            while (stream.Position < stream.Length)
            {
                ChunkSection chunkSection = new();
                await chunkSection.DeserializeAsync(stream);
                Sections.Add(chunkSection);

                BiomeSection biomeSection = new();
                await biomeSection.DeserializeAsync(stream);
                Biomes.Add(biomeSection);
            }
        }
        public async Task DeserializeAsync(Stream stream)
        {
            VarInt bSize = new();
            await bSize.DeserializeAsync(stream);

            byte[] buffer = new byte[bSize];
            await stream.ReadAsync(buffer);

            MemoryStream ms = new(buffer);
            await ReadSectionsAsync(ms);

            ms.Dispose();
        }

        async Task WriteSectionsAsync(Stream stream)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                await Sections[i].SerializeAsync(stream);
                await Biomes[i].SerializeAsync(stream);
            }
        }
        public async Task SerializeAsync(Stream stream)
        {
            MemoryStream ms = new();
            await WriteSectionsAsync(ms);

            VarInt bSize = new();
            bSize.Value = (int)ms.Length;

            await bSize.SerializeAsync(stream);
            await stream.WriteAsync(ms.GetDispose());
        }
    }
}
