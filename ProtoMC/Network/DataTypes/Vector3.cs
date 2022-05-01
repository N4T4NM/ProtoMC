using ProtoMC.Network.IO;

namespace ProtoMC.Network.DataTypes
{
    public class Vector3<T> : IDataType where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            X = (T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
            Y = (T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
            Z = (T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
        }

        public async Task SerializeAsync(Stream stream)
        {
            await PacketSerializer.SerializeStructAsync(X, stream);
            await PacketSerializer.SerializeStructAsync(Y, stream);
            await PacketSerializer.SerializeStructAsync(Z, stream);
        }
    }
}
