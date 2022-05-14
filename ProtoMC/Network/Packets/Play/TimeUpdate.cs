using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class TimeUpdate : IPacket
    {
        public ProtoHeader Header { get; } = new(0x59, IO.State.Play, Bound.Client);
        public long WorldAge { get; set; }
        public long TimeOfDay { get; set; }
    }
}
