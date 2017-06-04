using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Maps;
using System.Collections.Generic;

namespace Destiny.Network.Handler
{
    public static class MobHandler
    {
        public static void HandleMobMovement(MapleClient client, InPacket iPacket)
        {
            int objectID = iPacket.ReadInt();

            Mob mob;

            try
            {
                mob = client.Character.ControlledMobs[objectID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            short moveAction = iPacket.ReadShort();
            bool cheatResult = (iPacket.ReadByte() & 0xF) != 0;
            byte centerSplit = iPacket.ReadByte();
            int illegalVelocity = iPacket.ReadInt();
            byte unknown = iPacket.ReadByte();
            iPacket.ReadInt();

            Movements movements = Movements.Decode(iPacket);

            Movement lastMovement = movements[movements.Count - 1];

            mob.Position = lastMovement.Position;
            mob.Foothold = lastMovement.Foothold;
            mob.Stance = lastMovement.Stance;

            using (OutPacket oPacket = new OutPacket(SendOps.MobCtrlAck))
            {
                oPacket
                    .WriteInt(objectID)
                    .WriteShort(moveAction)
                    .WriteBool(cheatResult)
                    .WriteShort() // NOTE: Mob mana.
                    .WriteByte() // NOTE: Ability ID.
                    .WriteByte(); // NOTE: Ability level.

                client.Send(oPacket);
            }

            using (OutPacket oPacket = new OutPacket(SendOps.MobMove))
            {
                oPacket
                    .WriteInt(objectID)
                    .WriteBool(cheatResult)
                    .WriteByte(centerSplit)
                    .WriteInt(illegalVelocity);

                movements.Encode(oPacket);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }
    }
}
