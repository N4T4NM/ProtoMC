using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class ProtoArray<T> : IDataType
    {
        public bool Use32Bit { get; init; }
        //public VarInt Count { get; protected set; } = 0;
        public T[] Array { get; protected set; } = new T[0];

        public ProtoArray(T[] array, bool use32Bit = false)
        {
            Array = array;
            Use32Bit = use32Bit;
        }

        public T this[int index] => Array[index];

        public virtual async Task DeserializeAsync(Stream stream)
        {
            VarInt count = new();

            if (Use32Bit)
                count.Value = await stream.ReadStructAsync<int>();
            else await count.DeserializeAsync(stream);

            Array = new T[count];

            for (int i = 0; i < Array.Length; i++)
            {
                if (typeof(T).IsAssignableTo(typeof(IDataType)))
                {
                    Array[i] = Activator.CreateInstance<T>();
                    await ((IDataType)Array[i]!).DeserializeAsync(stream);
                }
                else
                    Array[i] = await stream.ReadStructAsync<T>(); //(T)await PacketSerializer.TypeToStructureAsync(typeof(T), stream);
            }
        }
        public virtual async Task SerializeAsync(Stream stream)
        {
            VarInt count = Array.Length;

            if (Use32Bit)
                await stream.WriteStructAsync(count.Value);
            else await count.SerializeAsync(stream);

            for (int i = 0; i < Array.Length; i++)
            {
                if (typeof(T).IsAssignableTo(typeof(IDataType)))
                    await ((IDataType)Array[i]!).SerializeAsync(stream);
                else
                    await stream.WriteStructAsync(Array[i]!);
            }
        }
    }
}
