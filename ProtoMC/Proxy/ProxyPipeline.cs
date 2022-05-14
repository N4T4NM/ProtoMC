using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets;
using System.Net.Sockets;

namespace ProtoMC.Proxy
{
    public class ProxyPipeline : IDisposable
    {
        public bool Working { get; protected set; }

        public ProxyClient? LocalClient { get; protected set; }
        public ProxyClient? RemoteClient { get; protected set; }

        public delegate void PacketReceivedEvent(ProxyClient sender, PacketCapturedEventArgs args);
        public delegate void PipelineErrorEvent(Exception ex);

        public event PacketReceivedEvent? PacketReceived;
        public event PipelineErrorEvent? PipelineError;

        public void CreatePipeline(Socket localClient, string host, int port)
        {
            //Generate local client
            //Connect with remote host
            //Start redirecting

            LocalClient = new(localClient);

            Socket remoteClient = new(SocketType.Stream, ProtocolType.Tcp);
            remoteClient.Connect(host, port);

            RemoteClient = new(remoteClient);

            Working = true;
            ReceiveServerBoundPacket();
            ReceiveClientBoundPacket();
        }

        protected void OnPipelineError(Exception ex)
        {
            if (!Working) return;
            PipelineError?.Invoke(ex);
            Dispose();
        }

        protected void ReceiveServerBoundPacket()
        {
            Task.Run(LocalClient!.ReadPacketAsync).ContinueWith(async (t) =>
            {
                try
                {
                    IPacket packet = await ((PacketData)await t).ConvertPacketAsync(
                    LocalClient!.CurrentState, Bound.Server);

                    PacketCapturedEventArgs args = new(packet);
                    PacketReceived?.Invoke(LocalClient!, args);

                    if (!args.Drop)
                    {
                        LocalClient!.ConfirmPacket(args.Packet);
                        await InjectPacketIntoServer(args.Packet);
                    }

                    ReceiveServerBoundPacket();
                }
                catch (Exception ex)
                {
                    OnPipelineError(ex);
                }
            });
        }
        protected void ReceiveClientBoundPacket()
        {
            Task.Run(RemoteClient!.ReadPacketAsync).ContinueWith(async (t) =>
            {
                try
                {
                    IPacket packet = await ((PacketData)await t).ConvertPacketAsync(
                    RemoteClient!.CurrentState, Bound.Client);

                    PacketCapturedEventArgs args = new(packet);
                    PacketReceived?.Invoke(RemoteClient!, args);

                    if (!args.Drop)
                    {
                        RemoteClient!.ConfirmPacket(args.Packet);
                        await InjectPacketIntoClient(args.Packet);
                    }

                    ReceiveClientBoundPacket();
                }
                catch (Exception ex)
                {
                    OnPipelineError(ex);
                }
            });
        }

        public async Task InjectPacketIntoClient(IPacket packet) 
            => await LocalClient!.WritePacketAsync(packet);
        public async Task InjectPacketIntoServer(IPacket packet)
            => await RemoteClient!.WritePacketAsync(packet);


        public void Dispose()
        {
            LocalClient?.Dispose();
            RemoteClient?.Dispose();

            Working = false;
        }
    }

    public class PacketCapturedEventArgs
    {
        public IPacket Packet { get; set; }
        public bool Drop { get; set; }

        public PacketCapturedEventArgs(IPacket packet)
        {
            Packet = packet;
            Drop = false;
        }
    }
}
