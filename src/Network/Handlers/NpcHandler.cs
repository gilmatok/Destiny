using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Life;
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

            // TODO: Implement movements.

            //using (OutPacket oPacket = new OutPacket(SendOps.NpcMove))
            //{
            //    oPacket
            //        .WriteInt(npc.ObjectID)
            //        .WriteByte(a)
            //        .WriteByte(b);

            //    client.Character.Map.Broadcast(oPacket);
            //}
        }
    }
}
