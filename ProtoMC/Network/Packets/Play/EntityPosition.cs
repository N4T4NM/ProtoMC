using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class EntityPosition : IPacket
    {
        public ProtoHeader Header { get; } = new(0x29, IO.State.Play, Bound.Client);

        public VarInt EntityID { get; set; } = new();
        public Vector3<short> DeltaPosition { get; set; } = new();
        public bool OnGround { get; set; }
    }
}
