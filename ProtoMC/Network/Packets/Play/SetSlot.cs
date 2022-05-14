using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class SetSlot : IPacket
    {
        public ProtoHeader Header { get; } = new(0x16, IO.State.Play, Bound.Client);
        public byte WindowID { get; set; }
        public VarInt StateID { get; set; } = new();
        public short Slot { get; set; }
        public SlotData Data { get; set; } = new();
    }
}
