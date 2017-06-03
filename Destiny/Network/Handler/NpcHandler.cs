using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Maps;
using Destiny.Network.Packet;
using System.Collections.Generic;

namespace Destiny.Network.Handler
{
    public static class NpcHandler
    {
        public static void HandleNpcMove(MapleClient client, InPacket iPacket)
        {
            int objectID = iPacket.ReadInt();
            Npc npc;

            try
            {
                npc = client.Character.ControlledNpcs[objectID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            byte a = iPacket.ReadByte();
            byte b = iPacket.ReadByte();
            Movements movements = null;

            if (npc.Data.IsMoving)
            {
                movements = Movements.Decode(iPacket);

                // TODO: Validate movements.

                return;
            }

            client.Character.Map.Broadcast(NpcPacket.NpcMovement(npc.ObjectID, a, b, movements));
        }
    }
}
