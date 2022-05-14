using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class PlayerRotation : IPacket
    {
        public ProtoHeader Header { get; } = new(0x13, IO.State.Play, Bound.Server);

        public Vector2<float> Rotation { get; set; } = new();
        public bool OnGround { get; set; }
    }
}
