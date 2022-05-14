using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class EntityPositionAndRotation : IPacket
    {
        public ProtoHeader Header { get; } = new(0x2A, IO.State.Play, Bound.Client);

        public VarInt EntityID { get; set; } = new();

        public Vector3<short> DeltaPosition { get; set; } = new();
        public Vector2<byte> Angle { get; set; } = new();

        public bool OnGround { get; set; }
    }
}
