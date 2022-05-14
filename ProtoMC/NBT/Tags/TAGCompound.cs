using ProtoMC.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ProtoMC.NBT.Tags
{
    public class TAGCompound : ITag, IDictionary<string, ITag>
    {
        readonly Dictionary<string, ITag> _tags = new();

        public ITag this[string key] { get => _tags[key]; set => _tags[key] = value; }
        public ICollection<string> Keys => _tags.Keys;
        public ICollection<ITag> Values => _tags.Values;

        public int Count => _tags.Count;
        public bool IsReadOnly => false;

        public void Add(string key, ITag value) => _tags.Add(key, value);
        public void Add(KeyValuePair<string, ITag> item) => _tags.Add(item.Key, item.Value);
        public void Clear() => _tags.Clear();

        public bool Contains(KeyValuePair<string, ITag> item) => _tags.Contains(item);
        public bool ContainsKey(string key) => _tags.ContainsKey(key);
        public void CopyTo(KeyValuePair<string, ITag>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<string, ITag>> GetEnumerator() => _tags.GetEnumerator();

        public bool Remove(string key) => _tags.Remove(key);
        public bool Remove(KeyValuePair<string, ITag> item) => _tags.Remove(item.Key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out ITag value) => _tags.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => _tags.GetEnumerator();

        public async Task ReadAsync(Stream stream)
        {
            byte id = await ITag.GetNextIDAsync(stream);
            while (id != 0)
            {
                Type type = ITag.TypeDefinitions[id];

                TAGString key = new();
                await key.ReadAsync(stream);

                ITag value = (ITag)type.Instantiate();
                await value.ReadAsync(stream);

                this.Add(key.Value, value);

                id = await ITag.GetNextIDAsync(stream);
            }
        }

        public async Task WriteAsync(Stream stream)
        {
            foreach(string key in this.Keys)
            {
                await stream.WriteAsync(new byte[1] { (byte)ITag.GetID(this[key].GetType()) }); //Type
                
                TAGString name = new() { Value = key };

                await name.WriteAsync(stream); //Key
                await this[key].WriteAsync(stream); //Value
            }
            await stream.WriteAsync(new byte[1] { 0 }); //TAG_End
        }
    }
}
