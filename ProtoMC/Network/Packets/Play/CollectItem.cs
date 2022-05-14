using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class CollectItem : IPacket
    {
        public ProtoHeader Header { get; } = new(0x61, IO.State.Play, Bound.Client);

        public VarInt CollectedID { get; set; } = 0;
        public VarInt CollectorID { get; set; } = 0;
        public VarInt ItemCount { get; set; } = 0;
    }
}
