namespace ProtoMC.Network.DataTypes
{
    public class ProtoRemainingBuffer : IDataType
    {
        public byte[] Value { get; set; } = new byte[0];

        public async Task DeserializeAsync(Stream stream)
        {
            Value = new byte[stream.Length - stream.Position];
            await stream.ReadAsync(Value, 0, Value.Length);
        }
        public async Task SerializeAsync(Stream stream) => await stream.WriteAsync(Value);

        public static implicit operator byte[](ProtoRemainingBuffer rb) => rb.Value;
        public static implicit operator ProtoRemainingBuffer(byte[] b) => new() { Value = b };
    }
}
