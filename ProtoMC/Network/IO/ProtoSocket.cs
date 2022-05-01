using ProtoMC.Network.Packets;
using ProtoMC.Network.Packets.Handshaking;
using ProtoMC.Network.Packets.Login;
using ProtoMC.Network.Packets.Play;
using System.Net.Sockets;

namespace ProtoMC.Network.IO
{
    public class ProtoSocket : IDisposable
    {
        public Socket BaseSocket { get; protected set; }
        public PacketStream? Stream { get; protected set; }
        public bool IsOwner { get; protected set; }

        public int CurrentEntityID { get; protected set; }
        public Guid CurrentUUID { get; protected set; }
        public string? CurrentUsername { get; protected set; }
        public string? CurrentHost { get; protected set; }
        public ushort CurrentPort { get; protected set; }

        public State CurrentState { get; protected set; }

        public ProtoSocket() : this(new(SocketType.Stream, ProtocolType.Tcp), true) { }
        public ProtoSocket(Socket socket, bool isOwner)
        {
            BaseSocket = socket;

            if (BaseSocket.Connected)
                Stream = new(new NetworkStream(BaseSocket));

            IsOwner = isOwner;
        }

        protected virtual void CheckPlayPacket(IPacket packet)
        {
            //TODO: Disconnect
            //throw new NotImplementedException();
            switch(packet)
            {
                case JoinGame join:
                    CurrentEntityID = join.EnitityID;
                    break;
            }
        }

        protected virtual void CheckStatusPacket(IPacket packet)
        {
            throw new NotImplementedException();
        }

        protected virtual void CheckLoginPacket(IPacket packet)
        {
            switch (packet)
            {
                case LoginStart login:
                    CurrentUsername = login.Name;
                    break;
                case SetCompression compression:
                    if (compression.Threshold > 0)
                        Stream = new CompressedPacketStream(Stream!.BaseStream, compression.Threshold);
                    break;
                case LoginSucess success:
                    CurrentUUID = success.UUID;
                    CurrentState = State.Play;
                    break;
            }
        }

        protected virtual void CheckHandshakePacket(IPacket packet)
        {
            switch (packet)
            {
                case Handshake handshake:
                    CurrentState = (State)handshake.NextState.Value;
                    CurrentHost = handshake.ServerAddress.Value;
                    CurrentPort = handshake.ServerPort;
                    break;
            }
        }

        protected virtual void CheckPacket(IPacket packet)
        {
            switch (packet.Header.State)
            {
                case State.Play:
                    CheckPlayPacket(packet);
                    break;
                case State.Status:
                    CheckStatusPacket(packet);
                    break;
                case State.Login:
                    CheckLoginPacket(packet);
                    break;
                case State.Handshaking:
                    CheckHandshakePacket(packet);
                    break;
            }
        }

        public virtual async Task WritePacketAsync(IPacket packet)
        {
            await Stream!.WriteAsync(packet);
            CheckPacket(packet);
        }
        public virtual async Task<IPacket> ReadPacketAsync()
        {
            PacketData data = await Stream!.ReadDataAsync();
            IPacket packet = await data.ConvertPacketAsync(CurrentState, DataTypes.Bound.Client);

            CheckPacket(packet);
            return packet;
        }

        public virtual void Dispose()
        {
            Stream?.Dispose();
            if (IsOwner) BaseSocket.Dispose();
        }
    }

    public enum State
    {
        Unknown = -1,
        Handshaking = 0,
        Status = 1,
        Login = 2,
        Play = 3
    }
}
