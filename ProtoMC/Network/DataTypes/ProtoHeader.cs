using ProtoMC.Network.IO;

namespace ProtoMC.Network.DataTypes
{
    public class ProtoHeader : IDataType
    {
        public VarInt Id { get; init; }
        public State State { get; init; }
        public Bound Bound { get; init; }

        public ProtoHeader(int id, State state, Bound bound)
        {
            Id = id;
            State = state;
            Bound = bound;
        }

        public async Task DeserializeAsync(Stream stream) => await Id.DeserializeAsync(stream);
        public async Task SerializeAsync(Stream stream) => await Id.SerializeAsync(stream);
    }

    public enum Bound
    {
        Unknown = -1,
        Client = 0,
        Server = 1
    }
}
