namespace ProtoMC.Network.DataTypes
{
    public class LightInfo : IDataType
    {
        public ProtoArray<byte> LightArray { get; set; } = new(new byte[0]);

        public async Task DeserializeAsync(Stream stream) => await LightArray.DeserializeAsync(stream);
        public async Task SerializeAsync(Stream stream) => await LightArray.SerializeAsync(stream);
    }
}
