using ProtoMC.Utils;

namespace ProtoMC.Network.DataTypes
{
    public class BitArray
    {
        public int Capacity { get; protected set; }
        public int BitsPerValue { get; protected set; }
        public int ValuesPerLong { get; protected set; }
        public int ValueMask { get; protected set; }

        int[] _buffer;
        public int BufferSize => _buffer.Length;

        public BitArray(int bitsPerValue, int capacity)
        {
            ValuesPerLong = (int)Math.Floor(64.0d / bitsPerValue);
            Capacity = capacity;
            BitsPerValue = bitsPerValue;
            ValueMask = (1 << BitsPerValue) - 1;

            int bufferSize = (int)(Math.Ceiling((double)Capacity / ValuesPerLong) * 2);
            _buffer = new int[bufferSize];
        }
        public async Task ReadStreamAsync(Stream stream)
        {
            for (int i = 0; i < BufferSize; i += 2)
            {
                _buffer[i + 1] = await stream.ReadStructAsync<int>();
                _buffer[i] = await stream.ReadStructAsync<int>();
            }
        }
        public async Task WriteToStreamAsync(Stream stream)
        {
            for (int i = 0; i < BufferSize; i += 2)
            {
                await stream.WriteStructAsync(_buffer[i + 1]);
                await stream.WriteStructAsync(_buffer[i]);
            }
        }
    }
}
