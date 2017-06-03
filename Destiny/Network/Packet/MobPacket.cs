using Destiny.Core.IO;
using Destiny.Game.Maps;

namespace Destiny.Network.Packet
{
    public static class MobPacket
    {
        public static byte[] MobEnterField(Mob mob)
        {
            return MobPacket.MobInternalPacket(mob, false);
        }

        public static byte[] MobControlRequest(Mob mob)
        {
            return MobPacket.MobInternalPacket(mob, true);
        }

        private static byte[] MobInternalPacket(Mob mob, bool requestControl)
        {
            using (OutPacket oPacket = new OutPacket(requestControl ? SendOps.MobChangeController : SendOps.MobEnterField))
            {
                if (requestControl)
                {
                    oPacket.WriteByte(1); // TODO: 2 if mob is provoked (aggro).
                }

                oPacket
                    .WriteInt(mob.ObjectID)
                    .WriteByte((byte)(mob.Controller == null ? 5 : 1))
                    .WriteInt(mob.MapleID)
                    .WriteZero(15) // NOTE: Unknown.
                    .WriteByte(0x88) // NOTE: Unknown.
                    .WriteZero(6) // NOTE: Unknown.
                    .WritePoint(mob.Position)
                    .WriteByte((byte)(0x02 | (mob.FacesLeft ? 0x01 : 0x00)))
                    .WriteShort(mob.Spawn.Foothold)
                    .WriteShort(mob.Foothold)
                    .WriteShort(-2) // NOTE: Spawn effect (-2 - new, -1 - existing).
                    .WriteByte(byte.MaxValue) // NOTE: Carnival team.
                    .WriteInt(); // NOTE: Unknown.

                return oPacket.ToArray();
            }
        }

        public static byte[] MobControlCancel(int objectID)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.MobChangeController))
            {
                oPacket
                    .WriteBool()
                    .WriteInt(objectID);

                return oPacket.ToArray();
            }
        }

        public static byte[] MobCtrlAck(int objectID, short moveAction, bool cheatResult, short mana, byte abilityID, byte abilityLevel)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.MobCtrlAck))
            {
                oPacket
                    .WriteInt(objectID)
                    .WriteShort(moveAction)
                    .WriteBool(cheatResult)
                    .WriteShort(mana)
                    .WriteByte(abilityID)
                    .WriteByte(abilityLevel);

                return oPacket.ToArray();
            }
        }
    }
}
