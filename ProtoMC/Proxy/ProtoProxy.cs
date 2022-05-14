using ProtoMC.Network.Packets;
using System.Net;
using System.Net.Sockets;

namespace ProtoMC.Proxy
{
    public class ProtoProxy
    {

        ProxyState _state = ProxyState.Closed;

        public bool Running { get; protected set; }
        public ProxyState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    StateChanged?.Invoke(value);
                }
            }
        }

        public Socket? ServerSocket { get; protected set; }

        public ProxyPipeline Pipeline { get; } = new();

        public delegate void StateChangedEvent(ProxyState state);
        public delegate void ProxyErrorEvent(Exception ex, bool fatal);

        public event StateChangedEvent? StateChanged;
        public event ProxyErrorEvent? ProxyError;

        public event ProxyPipeline.PacketReceivedEvent? PacketReceived;

        public ProtoProxy()
        {
            Pipeline.PipelineError += OnPipelineError;
            Pipeline.PacketReceived += OnPacketReceived;
        }

        protected void OnProxyError(Exception ex, bool fatal)
        {
            ProxyError?.Invoke(ex, fatal);
            if (fatal)
                StopProxy();
        }

        protected void OnPacketReceived(ProxyClient client, PacketCapturedEventArgs args)
            => PacketReceived?.Invoke(client, args);
        protected void OnPipelineError(Exception ex)
        {
            ProxyError?.Invoke(ex, false);
        }

        public async Task InjectPacketIntoClient(IPacket packet)
            => await Pipeline.InjectPacketIntoClient(packet);
        public async Task InjectPacketIntoServer(IPacket packet)
            => await Pipeline.InjectPacketIntoServer(packet);

        public void StartProxy(int bindPort, string remoteHost, int remotePort)
        {
            StopProxy();

            Running = true;
            State = ProxyState.Preparing;

            ServerSocket = new(SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, bindPort));
            ServerSocket.Listen(1);

            ListenerLoop(remoteHost, remotePort);
        }

        private void ListenerLoop(string host, int port)
        {
            while (Running)
            {
                State = ProxyState.Listening;
                Socket local = ServerSocket!.Accept();

                State = ProxyState.Connecting;
                try
                {
                    Pipeline.CreatePipeline(local, host, port);
                    State = ProxyState.Connected;
                    while (Pipeline.Working) ;
                }
                catch (Exception ex)
                {
                    OnProxyError(ex, true);
                }
            }
        }

        public void StopProxy()
        {
            if (State == ProxyState.Closed) return;

            State = ProxyState.Closing;
            Running = false;

            Pipeline.Dispose();

            ServerSocket?.Dispose();
            ServerSocket = null;

            State = ProxyState.Closed;
        }
    }

    public enum ProxyState
    {
        Closed,
        Closing,
        Preparing,
        Listening,
        Connecting,
        Connected
    }
}
