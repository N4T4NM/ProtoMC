// See https://aka.ms/new-console-template for more information
using ProtoMC.Network.DataTypes;
using ProtoMC.Network.IO;
using ProtoMC.Network.Packets;
using ProtoMC.Network.Packets.Handshaking;
using ProtoMC.Network.Packets.Login;
using ProtoMC.Network.Packets.Play;
using ProtoMC.Proxy;
using System.Net.Sockets;
using System.Text;

ProtoProxy proxy = new();

string BuildStr(IPacket packet)
{
    StringBuilder str = new();
    if (packet.GetType() == typeof(PacketData))
        str.Append($"[0x{packet.Header.Id.Value.ToString("X2")}] ");
    else str.Append($"[{packet.GetType().Name}] ");

    str.Append($"{packet.Header.State} | ");

    if (packet.Header.Bound == Bound.Server)
        str.Append($"C >> S");
    else str.Append($"C << S");

    return str.ToString();
}

List<byte[]> test = new();

proxy.ProxyError += (ex, fatal) =>
{
    Console.ForegroundColor = fatal ? ConsoleColor.Red : ConsoleColor.DarkMagenta;
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ForegroundColor = ConsoleColor.White;

    if (fatal)
        throw ex;
};
proxy.StateChanged += (state) =>
{
    Console.WriteLine($"[*] {state}");
};
proxy.PacketReceived += (sender, pak) =>
{
    if (pak.Packet.Header.Id == 0x66)
        pak.Drop = true;

    Console.WriteLine(BuildStr(pak.Packet));
};

proxy.StartProxy(4444, "127.0.0.1", 25565);