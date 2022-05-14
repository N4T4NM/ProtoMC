using ProtoMC.Network.DataTypes;
using ProtoMC.Utils;

namespace ProtoMC.Minecraft.World
{
    public class SingleValuePalette : IPalette
    {
        public VarInt Value { get; set; } = new();
        public byte BitsPerBlock { get; set; }

        public SingleValuePalette(byte bitsPerBlock)
        {
            BitsPerBlock = bitsPerBlock;
        }

        public async Task DeserializeAsync(Stream stream) => await Value.DeserializeAsync(stream);
        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync<byte>(0);
            await Value.SerializeAsync(stream);
            await stream.WriteStructAsync<byte>(0);
        }
    }
}
