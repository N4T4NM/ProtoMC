using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class ProtoString : IDataType
    {
        public string Value { get; set; } = "";

        public async Task DeserializeAsync(Stream stream)
        {
            VarInt sz = new();
            await sz.DeserializeAsync(stream);

            byte[] buffer = new byte[sz.Value];
            await stream.ReadAsync(buffer);

            Value = buffer.Decode();
        }
        public async Task SerializeAsync(Stream stream)
        {
            byte[] buffer = Value.Encode();
            await new VarInt() { Value = buffer.Length }.SerializeAsync(stream);
            await stream.WriteAsync(buffer);
        }

        public static implicit operator string(ProtoString pstr) => pstr.Value;
        public static implicit operator ProtoString(string str) => new() { Value = str };
    }
}
