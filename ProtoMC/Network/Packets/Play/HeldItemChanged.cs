using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class HeldItemChanged : IPacket
    {
        public ProtoHeader Header { get; } = new(0x48, IO.State.Play, Bound.Client);

        public byte Slot { get; set; }
    }
}
