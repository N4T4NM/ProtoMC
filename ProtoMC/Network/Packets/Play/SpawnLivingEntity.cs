using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class SpawnLivingEntity : IPacket
    {
        public ProtoHeader Header { get; } = new(0x02, IO.State.Play, Bound.Client);

        public VarInt EntityID { get; set; } = new();
        public Guid EntityUUID { get; set; }
        public VarInt Type { get; set; } = new();

        public Vector3<double> Position { get; set; } = new();
        public Vector3<byte> Angle { get; set; } = new();
        public Vector3<short> Velocity { get; set; } = new();
    }
}
