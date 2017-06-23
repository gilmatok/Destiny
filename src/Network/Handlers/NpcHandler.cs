using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Life;
using Destiny.Maple.Script;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static void HandleNpcConverse(MapleClient client, InPacket iPacket)
        {
            if (client.Character.NpcScript != null)
            {
                return;
            }

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

            if (!File.Exists(npc.ScriptPath))
            {
                Log.Warn("'{0}' tried to converse with an unimplemented npc {1}.", client.Character.Name, npc.MapleID);
            }
            else
            {
                client.Character.NpcScript = new NpcScript(npc, client.Character);

                try
                {
                    client.Character.NpcScript.Execute();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                finally
                {
                    client.Character.NpcScript = null;
                }
            }
        }
    }
}
