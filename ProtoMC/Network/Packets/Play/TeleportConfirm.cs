using ProtoMC.Network.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMC.Network.Packets.Play
{
    public class TeleportConfirm : IPacket
    {
        public ProtoHeader Header { get; } = new(0x00, IO.State.Play, Bound.Server);
        public VarInt TeleportID { get; set; } = 0;
    }
}
