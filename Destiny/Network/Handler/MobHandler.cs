using Destiny.Core.IO;
using Destiny.Game.Maps;
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
            bool cheatResult = (iPacket.ReadByte() & 0xF) != 0;
            byte centerSplit = iPacket.ReadByte();
            int illegalVelocity = iPacket.ReadInt();
            byte unknown = iPacket.ReadByte();
            iPacket.ReadInt();

            client.Send(MobPacket.MobCtrlAck(objectID, moveAction, cheatResult, 0, 0, 0));
        }
    }
}
