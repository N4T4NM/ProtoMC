using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Handshaking
{
    public class Handshake : IPacket
    {
        public ProtoHeader Header { get; } = new(0x00, IO.State.Handshaking, Bound.Server);

        public VarInt ProtocolVersion { get; set; } = new();
        public ProtoString ServerAddress { get; set; } = new();
        public ushort ServerPort { get; set; }
        public VarInt NextState { get; set; } = new();

        public Handshake(int protocolVersion, string address, ushort port, IO.State nextState)
        {
            ProtocolVersion.Value = protocolVersion;
            ServerAddress.Value = address;
            ServerPort = port;
            NextState.Value = (int)nextState;
        }
    }
}
