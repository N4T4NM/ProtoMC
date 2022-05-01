using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Login
{
    public class LoginSucess : IPacket
    {
        public ProtoHeader Header { get; } = new(0x02, IO.State.Login, Bound.Client);

        public Guid UUID { get; set; }
        public ProtoString Username { get; set; } = "";
    }
}
