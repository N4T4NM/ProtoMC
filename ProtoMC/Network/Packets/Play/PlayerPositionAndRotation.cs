using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class PlayerPositionAndRotation : IPacket
    {
        public ProtoHeader Header { get; } = new(0x12, IO.State.Play, Bound.Server);

        public Vector3<double> Position { get; set; } = new();
        public Vector2<float> Rotation { get; set; } = new();
        public bool OnGround { get; set; }
    }
}
