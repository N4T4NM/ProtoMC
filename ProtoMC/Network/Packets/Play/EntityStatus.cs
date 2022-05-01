using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class EntityStatus : IPacket
    {
        public ProtoHeader Header { get; } = new(0x1B, IO.State.Play, Bound.Client);
        public int EntityID { get; set; }
        public byte Status { get; set; }
    }
}
