using ProtoMC.Network.DataTypes;
using ProtoMC.Network.Packets.Play;
using ProtoMC.Proxy;

namespace ProtoMC.Test
{
    public class AimlockTest
    {
        public ProtoProxy Proxy { get; init; }
        public AimlockTest(ProtoProxy proxy)
        {
            Proxy = proxy;

            proxy.PacketReceived += OnPacketReceived;
        }

        Dictionary<int, Vector3<double>> entities = new();
        Vector3<double> position = new();

        void UpdatePositions(int id, Vector3<short> delta)
        {
            if (!entities.ContainsKey(id)) return;

            entities[id].X += delta.X;
            entities[id].Y += delta.Y;
            entities[id].Z += delta.Z;
        }

        double GetDistance(Vector3<double> a, Vector3<double> b)
        {
            double deltaX = b.X - a.X;
            double deltaY = b.Y - a.Y;
            double deltaZ = b.Z - a.Z;

            double deltaPow = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
            return Math.Sqrt(deltaPow);
        }

        int FindClosestEntity()
        {
            int id = -1;
            double distance = double.MaxValue;

            foreach (int key in entities.Keys)
            {
                double next = GetDistance(position, entities[key]);
                if (next < distance)
                {
                    distance = next;
                    id = key;
                }
            }

            return id;
        }
        void Lock(int entity)
        {
            Proxy.InjectPacketIntoClient(new FacePlayer()
            {
                AimWith = 1,
                TargetPart = 1,
                EntityID = entity,
                Target = entities[entity],
                IsEntity = true
            }).GetAwaiter().GetResult();
        }

        void DoAimlock()
        {
            int entity = FindClosestEntity();
            if (entity == -1) return;

            Lock(entity);
        }

        private void OnPacketReceived(ProxyClient sender, PacketCapturedEventArgs args)
        {
            switch (args.Packet)
            {
                case DestroyEntities dest:
                    foreach (VarInt id in dest.Entities.Array)
                        if (entities.ContainsKey(id))
                            entities.Remove(id);
                    break;

                case SpawnLivingEntity spwn:

                    if (!entities.ContainsKey(spwn.EntityID))
                        entities.Add(spwn.EntityID, spwn.Position);

                    break;

                case PlayerPosition pos:
                    position = pos.Position;
                    break;

                case PlayerPositionAndRotation posRot:
                    position = posRot.Position;
                    break;

                case EntityPosition pos:
                    UpdatePositions(pos.EntityID, pos.DeltaPosition);
                    break;
                case EntityPositionAndRotation posRot:
                    UpdatePositions(posRot.EntityID, posRot.DeltaPosition);
                    break;
            }

            DoAimlock();
        }
    }
}
