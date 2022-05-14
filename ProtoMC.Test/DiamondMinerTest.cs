using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets.Play;
using ProtoMC.Proxy;

namespace ProtoMC.Test
{
    public class DiamondMinerTest
    {
        public ProtoProxy Proxy { get; init; }
        public DiamondMinerTest(ProtoProxy proxy)
        {
            Proxy = proxy;
            proxy.PacketReceived += OnPacketReceived;
        }

        Vector3<double>? TargetBlock = null;

        void Tick()
        {

        }

        private void OnPacketReceived(ProxyClient sender, PacketCapturedEventArgs args)
        {
            switch (args.Packet)
            {
                case HeldItemChanged hic:
                    break;

                case SetSlot ss:
                    break;
            }

            Tick();
        }
    }
}
