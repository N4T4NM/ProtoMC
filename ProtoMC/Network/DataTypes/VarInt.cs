namespace ProtoMC.Network.DataTypes
{
    public class VarInt : IDataType
    {
        public const int SEGMENT_BITS = 0x7F;
        public const int CONTINUE_BIT = 0x80;

        public int Value { get; set; }

        public async Task DeserializeAsync(Stream stream)
        {
            byte[] buffer = new byte[1];
            Value = 0;
            int position = 0;

            while (true)
            {
                await stream.ReadAsync(buffer);
                byte b = buffer[0];

                Value |= (b & SEGMENT_BITS) << position;
                if ((b & CONTINUE_BIT) == 0) break;

                position += 7;
                if (position >= 32) throw new InvalidDataException("Not a VarInt !");
            }
        }

        public async Task SerializeAsync(Stream stream)
        {
            int val = Value;
            while (true)
            {
                if ((val & ~SEGMENT_BITS) == 0)
                {
                    await stream.WriteAsync(new byte[1] { (byte)val });
                    break;
                }

                await stream.WriteAsync(new byte[1] { (byte)((val & SEGMENT_BITS) | CONTINUE_BIT) });
                val >>= 7;
            }
        }

        public static implicit operator int(VarInt vi) => vi.Value;
        public static implicit operator VarInt(int i) => new() { Value = i };
    }
}
