using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets;
using ProtoMC.Utils;
using System.IO.Compression;

namespace ProtoMC.Network.IO
{
    public class CompressedPacketStream : PacketStream
    {
        public int Threshold { get; init; }
        public CompressedPacketStream(Stream baseStream, int threshold) : base(baseStream)
        {
            Threshold = threshold;
        }

        async Task<byte[]> CompressAsync(byte[] packetContent)
        {
            MemoryStream output = new();

            ZLibStream zlib = new(output, CompressionMode.Compress);
            await zlib.WriteAsync(packetContent, 0, packetContent.Length);
            await zlib.FlushAsync();

            zlib.Close();
            byte[] result = output.GetDispose();

            zlib.Dispose();
            return result;
        }
        public override async Task WriteAsync(IPacket packet)
        {
            byte[] packetContent = await PacketSerializer.SerializePacketAsync(packet);
            VarInt dataLength = packetContent.Length;

            if (dataLength.Value < Threshold)
                dataLength.Value = 0;
            else
                packetContent = await CompressAsync(packetContent);

            MemoryStream fullPacket = new();
            await dataLength.SerializeAsync(fullPacket);
            await fullPacket.WriteAsync(packetContent);


            MemoryStream networkPacket = new();

            VarInt packetLength = (int)fullPacket.Length;
            await packetLength.SerializeAsync(networkPacket);
            await networkPacket.WriteAsync(fullPacket.GetDispose());

            await BaseStream.WriteAsync(networkPacket.GetDispose());
        }

        async Task<Stream> DecompressAsync(byte[] compressed, int sz)
        {
            MemoryStream input = new(compressed);
            ZLibStream zlib = new(input, CompressionMode.Decompress);

            byte[] result = new byte[sz];

            await zlib.ReadAsync(result);
            zlib.Dispose();

            return new MemoryStream(result);
        }
        public override async Task<PacketData> ReadDataAsync()
        {
            Stream packet = await ReadPacketBufferAsync();

            VarInt dataLength = new();
            await dataLength.DeserializeAsync(packet);

            if (dataLength.Value == 0)
            {
                VarInt uncId = new();
                await uncId.DeserializeAsync(packet);

                byte[] uncData = new byte[packet.Length - packet.Position];
                await packet.ReadAsync(uncData);
                packet.Dispose();

                return new(uncId, uncData);
            }

            byte[] fullPacket = new byte[packet.Length - packet.Position];
            await packet.ReadAsync(fullPacket);
            packet.Dispose();

            Stream dec = await DecompressAsync(fullPacket, dataLength);

            VarInt id = new();
            await id.DeserializeAsync(dec);

            byte[] data = new byte[dec.Length - dec.Position];
            await dec.ReadAsync(data);

            dec.Dispose();
            return new(id, data);
        }
    }
}
