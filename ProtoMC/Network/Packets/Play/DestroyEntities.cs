using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class DestroyEntities : IPacket
    {
        public ProtoHeader Header { get; } = new(0x3A, IO.State.Play, Bound.Client);
        public ProtoArray<VarInt> Entities { get; set; } = new ProtoArray<VarInt>(new VarInt[0]);
    }
}
