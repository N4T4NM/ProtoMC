using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class PlayerPosition : IPacket
    {
        public ProtoHeader Header { get; } = new(0x11, IO.State.Play, Bound.Server);

        public Vector3<double> Position { get; set; } = new();
        public bool OnGround { get; set; }
    }
}
