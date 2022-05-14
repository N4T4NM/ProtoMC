using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class SlotData : IDataType
    {
        public bool Present { get; set; }
        public VarInt? ItemID { get; set; }
        public byte? ItemCount { get; set; } = null;
        public NBTField? NBT { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            Present = await stream.ReadStructAsync<bool>();
            if (!Present) return;

            ItemID = new();
            await ItemID.DeserializeAsync(stream);

            byte[] b = new byte[1];
            await stream.ReadAsync(b);
            ItemCount = b[0];

            NBT = new();
            await NBT.DeserializeAsync(stream);
        }

        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteStructAsync(Present);
            if (!Present) return;

            await ItemID!.SerializeAsync(stream);
            await stream.WriteStructAsync(ItemCount!.Value);
            await NBT!.SerializeAsync(stream);
        }
    }
}
