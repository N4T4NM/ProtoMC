using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class BlockEntity : IDataType
    {
        public byte PacketXZ { get; set; }
        public short Y { get; set; }
        public VarInt Type { get; set; } = 0;
        public NBTField Data { get; set; } = new();

        public async Task DeserializeAsync(Stream stream)
        {
            byte[] buffer = new byte[1];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            PacketXZ = buffer[0];
            Y = await stream.ReadStructAsync<short>();
            await Type.DeserializeAsync(stream);
            await Data.DeserializeAsync(stream);
        }
        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteAsync(new byte[1] { PacketXZ });
            await stream.WriteStructAsync(Y);
            await Type.SerializeAsync(stream);
            await Data.SerializeAsync(stream);
        }
    }
}
