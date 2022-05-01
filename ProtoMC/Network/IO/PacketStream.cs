using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets;
using ProtoMC.Utils;

namespace ProtoMC.Network.IO
{
    public class PacketStream : IDisposable
    {
        public Stream BaseStream { get; init; }

        public PacketStream(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public virtual async Task WriteAsync(IPacket packet)
        {
            byte[] data = await PacketSerializer.SerializePacketAsync(packet);
            VarInt sz = new() { Value = data.Length };

            MemoryStream ms = new();
            await sz.SerializeAsync(ms);
            await ms.WriteAsync(data);

            byte[] netBuffer = ms.GetDispose();
            await BaseStream.WriteAsync(netBuffer);
        }

        protected virtual async Task<Stream> ReadPacketBufferAsync()
        {
            VarInt sz = new();
            await sz.DeserializeAsync(BaseStream);

            byte[] packet = new byte[sz.Value];
            int offset = 0;
            while (offset < packet.Length)
                offset += await BaseStream.ReadAsync(packet, offset, packet.Length - offset);

            return new MemoryStream(packet);
        }
        public virtual async Task<PacketData> ReadDataAsync()
        {
            Stream packet = await ReadPacketBufferAsync();

            VarInt id = new();
            await id.DeserializeAsync(packet);

            byte[] buffer = new byte[packet.Length - packet.Position];
            await packet.ReadAsync(buffer);
            packet.Dispose();

            return new(id, buffer);
        }

        public void Dispose() => BaseStream.Dispose();
    }
}
