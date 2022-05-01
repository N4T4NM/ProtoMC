using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Login
{
    public class LoginStart : IPacket
    {
        public ProtoHeader Header { get; } = new(0x00, IO.State.Login, Bound.Server);
        public ProtoString Name { get; set; }

        public LoginStart(string name)
        {
            Name = name;
        }
    }
}
