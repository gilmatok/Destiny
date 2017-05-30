using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network;

namespace Destiny.Packet
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
            using (OutPacket oPacket = new OutPacket(requestControl ? SendOpcode.NpcChangeController : SendOpcode.NpcEnterField))
            {
                if (requestControl)
                {
                    oPacket.WriteBool(true);
                }

                oPacket
                    .WriteInt(npc.ObjectID)
                    .WriteInt(npc.MapleID)
                    .WritePoint(npc.Position)
                    .WriteBool(!npc.Flip)
                    .WriteShort(npc.Foothold)
                    .WriteShort(npc.MinimumClickX)
                    .WriteShort(npc.MaximumClickX)
                    .WriteBool(!npc.Hide);

                return oPacket.ToArray();
            }
        }

        public static byte[] NpcControlCancel(int objectID)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.NpcChangeController))
            {
                oPacket
                    .WriteBool()
                    .WriteInt(objectID);

                return oPacket.ToArray();
            }
        }
    }
}
