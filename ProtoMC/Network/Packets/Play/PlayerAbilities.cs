using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class PlayerAbilities : IPacket
    {
        public ProtoHeader Header { get; } = new(0x32, IO.State.Play, Bound.Client);

        public PlayerAbility Abilities { get; set; } = new();
        public float FlyingSpeed { get; set; }
        public float FOVMod { get; set; }
    }
}
