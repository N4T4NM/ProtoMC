using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class SpawnEntity : IPacket
    {
        public ProtoHeader Header { get; } = new(0x00, IO.State.Play, Bound.Client);

        public VarInt EntityID { get; set; } = new();
        public Guid UUID { get; set; }
        public VarInt Type { get; set; } = new();

        public Vector3<double> Position { get; set; } = new();
        public Vector2<byte> Angle { get; set; } = new();

        public int Data { get; set; }

        public Vector3<short> Velocity { get; set; } = new();
    }
}
