namespace ProtoMC.Network.DataTypes
{
    public class PlayerAbility : IDataType
    {
        public byte Bits { get; set; }
        public async Task DeserializeAsync(Stream stream)
        {
            byte[] buffer = new byte[1];
            await stream.ReadAsync(buffer);

            Bits = buffer[0];
        }

        public async Task SerializeAsync(Stream stream)
            => await stream.WriteAsync(new byte[1] { Bits });

        public bool GetFlag(AbilityFlags flag)
            => (Bits & (byte)flag) != 0;

        public void SetFlag(AbilityFlags flag, bool value)
        {
            if (value)
                Bits |= (byte)flag;
            else Bits &= (byte)~flag;
        }
    }

    [Flags]
    public enum AbilityFlags : byte
    {
        Invulnerable = 0x01,
        Flying = 0x02,
        AllowFlying = 0x04,
        CreativeMode = 0x08
    }
}
