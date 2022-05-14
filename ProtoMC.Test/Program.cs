// See https://aka.ms/new-console-template for more information
using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets;
using ProtoMC.Network.Packets.Login;
using ProtoMC.Network.Packets.Play;
using ProtoMC.Proxy;
using ProtoMC.Test;
using System.Text;

//TagCompound compound = new();


//MemoryStream ms = new();

//GZipStream zlib = new(new MemoryStream(File.ReadAllBytes(@"D:\Tools\Minecraft\Server 1.18.2\world\level.dat")), CompressionMode.Decompress);
//zlib.CopyTo(ms);
//zlib.Dispose();

//ms.Position = 0;
//await compound.DeserializeAsync(ms);
//return;

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

new DiamondMinerTest(proxy);

proxy.ProxyError += (ex, fatal) =>
{
    if (!fatal && ex is IOException) return;

    Console.ForegroundColor = fatal ? ConsoleColor.Red : ConsoleColor.DarkMagenta;
    Console.WriteLine($"[ERROR] {ex}");
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
    switch (pak.Packet)
    {
        case LoginStart login:
            login.Name = "TestAccount";
            break;

        case ChunkDataAndLight chunk:
            break;
    }

    if (pak.Packet.Header.Id == 0x66)
        pak.Drop = true;

    Console.WriteLine(BuildStr(pak.Packet));
};

proxy.StartProxy(4444, "127.0.0.1", 25565);