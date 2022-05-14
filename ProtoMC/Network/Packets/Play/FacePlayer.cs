using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class FacePlayer : IPacket
    {
        public ProtoHeader Header { get; } = new(0x37, IO.State.Play, Bound.Client);

        public VarInt AimWith { get; set; } = new();

        public Vector3<double> Target { get; set; } = new();
        public bool IsEntity { get; set; }

        public VarInt EntityID { get; set; } = new();
        public VarInt TargetPart { get; set; } = new();
    }
}
