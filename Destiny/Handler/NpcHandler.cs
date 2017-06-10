using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Maps;
using System.Collections.Generic;

namespace Destiny.Handler
{
    public static class NpcHandler
    {
        public static void HandleNpcMovement(MapleClient client, InPacket iPacket)
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

            using (OutPacket oPacket = new OutPacket(SendOps.NpcMove))
            {
                oPacket
                    .WriteInt(npc.ObjectID)
                    .WriteByte(a)
                    .WriteByte(b);

                if (npc.Data.IsMoving)
                {
                    movements = Movements.Decode(iPacket);

                    movements.Encode(oPacket);
                }

                client.Character.Map.Broadcast(oPacket);
            }
        }
    }
}
