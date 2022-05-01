using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class JoinGame : IPacket
    {
        public ProtoHeader Header { get; } = new(0x26, IO.State.Play, Bound.Client);

        public int EnitityID { get; set; }
        public ProtoRemainingBuffer Remaining { get; set; } = new();
        //TODO: add all fields
    }
}
