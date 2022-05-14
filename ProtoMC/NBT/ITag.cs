using ProtoMC.NBT.Tags;

namespace ProtoMC.NBT
{
    public interface ITag
    {
        public Task ReadAsync(Stream stream);
        public Task WriteAsync(Stream stream);

        public static readonly IReadOnlyDictionary<int, Type> TypeDefinitions = new Dictionary<int, Type>()
        {
            { 2, typeof(TAGShort) },
            { 8, typeof(TAGString) },
            { 10, typeof(TAGCompound) },
            { 12, typeof(TAGArray<long>) }
        };

        public static async Task<byte> GetNextIDAsync(Stream stream)
        {
            byte[] buffer = new byte[1];
            await stream.ReadAsync(buffer, 0, 1);

            return buffer[0];
        }

        public static int GetID(Type type)
        {
            foreach (int key in TypeDefinitions.Keys)
                if (TypeDefinitions[key] == type)
                    return key;

            return -1;
        }
    }
}
