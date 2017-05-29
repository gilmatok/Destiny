using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;

namespace Destiny.Packet
{
    public static class NpcPacket
    {
        public static byte[] NpcEnterField(Npc npc)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.NpcEnterField))
            {
                oPacket
                    .WriteInt(npc.ObjectID)
                    .WriteInt(npc.MapleID)
                    .WritePoint(npc.Position)
                    .WriteBool(!npc.Flip)
                    .WriteShort(npc.Foothold)
                    .WriteShort(npc.MinClickPos)
                    .WriteShort(npc.MaxClickPos)
                    .WriteBool(true); // TODO: bHide.

                return oPacket.ToArray();
            }
        }
    }
}
