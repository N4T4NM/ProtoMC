namespace ProtoMC.Network.DataTypes
{
    public interface IDataType
    {
        public Task SerializeAsync(Stream stream);
        public Task DeserializeAsync(Stream stream);
    }
}
