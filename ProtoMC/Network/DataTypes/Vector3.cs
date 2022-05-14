using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class Vector3<T> : IDataType where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            X = await stream.ReadStructAsync<T>();
            Y = await stream.ReadStructAsync<T>();
            Z = await stream.ReadStructAsync<T>();
        }

        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(X);
            await stream.WriteStructAsync(Y);
            await stream.WriteStructAsync(Z);
        }
    }
}
