using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network;

namespace Destiny.Packet
{
    public static class MobPacket
    {
        public static byte[] MobEnterField(Mob mob)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.MobEnterField))
            {
                oPacket
                    .WriteInt(mob.ObjectID)
                    .WriteByte(5) // TODO: 1 if mob is being controlled.
                    .WriteInt(mob.MapleID)
                    .WriteZero(15) // NOTE: Unknown.
                    .WriteByte(0x88) // NOTE: Unknown.
                    .WriteZero(6) // NOTE: Unknown.
                    .WritePoint(mob.Position)
                    .WriteByte(mob.Stance)
                    .WriteShort() // NOTE: Original foothold.
                    .WriteShort(mob.Foothold)
                    .WriteShort(-2) // NOTE: Spawn effect (-2 - new, -1 - existing).
                    .WriteByte(byte.MaxValue) // NOTE: Carnival team.
                    .WriteInt(); // NOTE: Unknown.

                return oPacket.ToArray();
            }
        }
    }
}
