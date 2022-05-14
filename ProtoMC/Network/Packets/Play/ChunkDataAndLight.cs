using ProtoMC.Network.DataTypes;

namespace ProtoMC.Network.Packets.Play
{
    public class ChunkDataAndLight : IPacket
    {
        public ProtoHeader Header { get; } = new(0x22, IO.State.Play, Bound.Client);

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        public NBTField Heightmaps { get; set; } = new();

        //public ChunkData Data { get; set; } = new();
        //public ProtoArray<BlockEntity> BlockEntities { get; set; } = new(new BlockEntity[0]);

        //public bool TrustEdges { get; set; }

        //public byte SkyLightMask { get; set; }
        //public byte BlockLightMask { get; set; }

        //public byte EmptySkyLightMask { get; set; }
        //public byte EmptyBlockLightMask { get; set; }

        //public ProtoArray<LightInfo> SkyLights { get; set; } = new(new LightInfo[0]);
        //public ProtoArray<LightInfo> BlockLights { get; set; } = new(new LightInfo[0]);

        public ProtoRemainingBuffer NotImplementedData { get; set; } = new();
    }
}
