using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Login
{
    public class SetCompression : IPacket
    {
        public ProtoHeader Header { get; } = new(0x03, IO.State.Login, Bound.Client);
        public VarInt Threshold { get; set; } = 0;
    }
}
