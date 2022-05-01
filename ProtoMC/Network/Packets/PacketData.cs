using ProtoMC.Network.DataTypes;
using ProtoMC.Network.IO;
using System.Reflection;

namespace ProtoMC.Network.Packets
{
    public class PacketData : IPacket
    {
        public ProtoHeader Header { get; set; } = new(-1, State.Unknown, Bound.Unknown);
        public ProtoRemainingBuffer Data { get; set; } = new byte[0];

        public PacketData(int id, byte[] data)
        {
            Header = new(id, State.Unknown, Bound.Unknown);
            Data = data;
        }

        public static IPacket InstantiatePacket(Type type)
        {
            ConstructorInfo ctor = type.GetConstructors()[0];
            ParameterInfo[] ctorParams = ctor.GetParameters();

            object?[] paramsInfo = new object?[ctorParams.Length];
            for (int i = 0; i < ctorParams.Length; i++)
            {
                ParameterInfo param = ctorParams[i];
                if (param.ParameterType.IsValueType)
                    paramsInfo[i] = Activator.CreateInstance(param.ParameterType);
            }

            return (IPacket)Activator.CreateInstance(type, paramsInfo)!;
        }
        public async Task<IPacket> ConvertPacketAsync(State state, Bound bound)
        {
            Header = new(Header.Id, state, bound);

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsAssignableTo(typeof(IPacket)) && t != typeof(IPacket))
                {
                    IPacket packet = InstantiatePacket(t);
                    if (packet.Header.Id.Value == Header.Id.Value &&
                        packet.Header.State == state && packet.Header.Bound == bound)
                    {
                        await PacketSerializer.DeserializeDataIntoPacketAsync(Data, packet);
                        return packet;
                    }
                }
            }

            return this;
        }
    }
}
