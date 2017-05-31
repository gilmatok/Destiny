using Destiny.Core.IO;
using Destiny.Game.Maps;

namespace Destiny.Network.Packet
{
    public static class NpcPacket
    {
        public static byte[] NpcEnterField(Npc npc)
        {
            return NpcPacket.NpcInternalPacket(npc, false);
        }

        public static byte[] NpcControlRequest(Npc npc)
        {
            return NpcPacket.NpcInternalPacket(npc, true);
        }

        private static byte[] NpcInternalPacket(Npc npc, bool requestControl)
        {
            using (OutPacket oPacket = new OutPacket(requestControl ? SendOps.NpcChangeController : SendOps.NpcEnterField))
            {
                if (requestControl)
                {
                    oPacket.WriteBool(true);
                }

                oPacket
                    .WriteInt(npc.ObjectID)
                    .WriteInt(npc.MapleID)
                    .WritePoint(npc.Position)
                    .WriteBool(!npc.Spawn.Flip)
                    .WriteShort(npc.Spawn.Foothold)
                    .WriteShort(npc.Spawn.MinimumClickX)
                    .WriteShort(npc.Spawn.MaximumClickX)
                    .WriteBool(!npc.Spawn.Hide);

                return oPacket.ToArray();
            }
        }

        public static byte[] NpcControlCancel(int objectID)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.NpcChangeController))
            {
                oPacket
                    .WriteBool()
                    .WriteInt(objectID);

                return oPacket.ToArray();
            }
        }
    }
}
