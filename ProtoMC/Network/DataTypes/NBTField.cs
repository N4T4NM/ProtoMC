using ProtoMC.NBT;
using ProtoMC.NBT.Tags;

namespace ProtoMC.Network.DataTypes
{
    public class NBTField : IDataType
    {
        public TAGCompound Value { get; set; } = new();

        public async Task DeserializeAsync(Stream stream)
        {
            Value = new();

            byte check = await ITag.GetNextIDAsync(stream);
            if (check == 0) 
                return;

            if (check != 10) 
                throw new InvalidDataException("Expected TAGCompound");

            byte[] nameLen = new byte[2];
            await stream.ReadAsync(nameLen);

            if (nameLen[0] > 0 || nameLen[1] > 0) throw new InvalidDataException("Empty string expected !");
            await Value.ReadAsync(stream);
        }
        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteAsync(new byte[3] {
                (byte)ITag.GetID(typeof(TAGCompound)), 0, 0 }); //TAG_Compound + Null name
            await Value.WriteAsync(stream);
        }
    }
}
