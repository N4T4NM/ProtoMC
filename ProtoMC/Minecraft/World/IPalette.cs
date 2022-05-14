namespace ProtoMC.Minecraft.World
{
    public interface IPalette
    {
        public byte BitsPerBlock { get; set; }
        public Task DeserializeAsync(Stream stream);
        public Task SerializeAsync(Stream stream);
    }
}
