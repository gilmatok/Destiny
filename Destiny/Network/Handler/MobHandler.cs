using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Maps;
using Destiny.Network;
using Destiny.Network.Packet;
using System.Collections.Generic;

namespace Destiny.Network.Handler
{
    public static class MobHandler
    {
        public static void OnMobMove(MapleClient client, InPacket iPacket)
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
            bool isUsingAbility = iPacket.ReadBool();
            byte usingAbility = iPacket.ReadByte();
            Point projectileTarget = iPacket.ReadPoint();
            iPacket.Skip(17);

            int rewindOffset = iPacket.Position;

            client.Character.Map.DecodeMovement(mob, iPacket);

            iPacket.Position = rewindOffset;

            client.Send(MobPacket.MobCtrlAck(objectID, moveAction, isUsingAbility, 0));

            // TODO map.
        }
    }
}
