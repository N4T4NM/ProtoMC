using ProtoMC.Network.IO;

namespace ProtoMC.Network.DataTypes
{
    public class Vector2<T> : IDataType where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            X = (T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
            Y = (T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
        }

        public async Task SerializeAsync(Stream stream)
        {
            await PacketSerializer.SerializeStructAsync(X, stream);
            await PacketSerializer.SerializeStructAsync(Y, stream);
        }
    }
}
