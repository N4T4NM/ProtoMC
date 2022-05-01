using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class ActionBar : IPacket
    {
        public ProtoHeader Header { get; } = new(0x41, IO.State.Play, Bound.Client);
        public ProtoString Text { get; set; } = "";
    }
}
