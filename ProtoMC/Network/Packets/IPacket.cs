using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets
{
    public interface IPacket
    {
        public ProtoHeader Header { get; }
    }
}
