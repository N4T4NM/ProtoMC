using ProtoMC.Network.IO;
using ProtoMC.Network.Packets;
using System.Net.Sockets;

namespace ProtoMC.Proxy
{
    public class ProxyClient : ProtoSocket
    {
        public ProxyClient(Socket socket) : base(socket, true) { }

        public override async Task<IPacket> ReadPacketAsync()
        {
            PacketData packet = await Stream!.ReadDataAsync();
            return packet;
        }     
        public void ConfirmPacket(IPacket packet) => CheckPacket(packet);
    }
}
