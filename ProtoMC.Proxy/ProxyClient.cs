using ProtoMC.Network;
using System.Net;
using System.Net.Sockets;

namespace ProtoMC.Proxy
{
    public class ProxyClient : Client
    {
        ProxyConnectionState _state = ProxyConnectionState.Closed;

        public ProxyConnectionState State
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }

        public Socket? Server { get; protected set; }
        public Client? LocalClient { get; protected set; }

        public delegate void StateChangedEvent(ProxyConnectionState state);
        public event StateChangedEvent? StateChanged;

        public delegate void ServerErrorEvent(Exception ex, bool fatalError);
        public event ServerErrorEvent? ServerError;

        protected void OnServerError(Exception ex, bool fatalError)
        {
            if(fatalError)
            {
                Server?.Dispose();
                LocalClient?.Dispose();

                Dispose();

                Socket = new(SocketType.Stream, ProtocolType.Tcp);
                State = ProxyConnectionState.Closed;
            }

            ServerError?.Invoke(ex, fatalError);
        }

        public delegate void PacketCapturedEvent(IPacket packet, ref bool drop);

        public event PacketCapturedEvent? OutgoingPacketCaptured;
        public event PacketCapturedEvent? IncomingPacketCaptured;

        async Task BeginPacketExchange()
        {
            try
            {
                LocalClient!.ReceivingPacket += OnLocalPacketCaptured;
                ReceivingPacket += OnRemotePacketCaptured;

                Task<IPacket> localRecv = LocalClient!.ReceivePacketAsync();
                Task<IPacket> remoteRecv = ReceivePacketAsync();

                while (true)
                {
                    if (remoteRecv.IsCompleted)
                    {
                        await remoteRecv;
                        remoteRecv = ReceivePacketAsync();
                    }

                    if (localRecv.IsCompleted)
                    {
                        await localRecv;
                        localRecv = LocalClient!.ReceivePacketAsync();
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnRemotePacketCaptured(IPacket packet, ref bool dropPacket)
            => IncomingPacketCaptured?.Invoke(packet, ref dropPacket);
        private void OnLocalPacketCaptured(IPacket packet, ref bool dropPacket) 
            => OutgoingPacketCaptured?.Invoke(packet, ref dropPacket);

        async Task WaitForClient(int bindPort, string remoteHost, int remotePort)
        {
            try
            {
                Server = new(SocketType.Stream, ProtocolType.Tcp);
                Server.Bind(new IPEndPoint(IPAddress.Any, bindPort));
                Server.Listen(1);

                State = ProxyConnectionState.Waiting;
                LocalClient = new(await Server.AcceptAsync());

                State = ProxyConnectionState.Connecting;
                await OpenConnectionAsync(remoteHost, remotePort);
                await BeginPacketExchange();
            } catch(Exception ex)
            {
                OnServerError(ex, true);
                return;
            }
        }

        public void StartProxy(int bindPort, string remoteHost, int remotePort)
        {
            new Task(async () =>
            {
                await WaitForClient(bindPort, remoteHost, remotePort);
            }).Start();
        }
    }

    public enum ProxyConnectionState
    {
        Closed,
        Waiting,
        Connecting,
        Connected
    }
}