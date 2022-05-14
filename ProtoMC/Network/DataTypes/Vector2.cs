using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class Vector2<T> : IDataType where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            X = await stream.ReadStructAsync<T>();
            Y = await stream.ReadStructAsync<T>();
        }

        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(X);
            await stream.WriteStructAsync(Y);
        }
    }
}
