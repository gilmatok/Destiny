using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Maps;
using Destiny.Server;
using System.Collections.Generic;

namespace Destiny.Network.Handler
{
    public static class PlayerHandler
    {
        public static void HandleMapChange(MapleClient client, InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != client.Character.Portals)
            {
                return;
            }

            int destinationID = iPacket.ReadInt();

            switch (destinationID)
            {
                case -1:
                    {
                        string label = iPacket.ReadMapleString();
                        Portal portal;

                        try
                        {
                            portal = client.Character.Map.Portals[label];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        client.Character.ChangeMap(portal.Data.DestinationMap, portal.Link.ID);
                    }
                    break;
            }
        }

        public static void HandlePlayerChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                MasterServer.Instance.Commands.Execute(client.Character, text);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(SendOps.UserChat))
                {
                    oPacket
                        .WriteInt(client.Character.ID)
                        .WriteBool(client.Character.IsGm)
                        .WriteMapleString(text)
                        .WriteBool(shout);

                    client.Character.Map.Broadcast(oPacket);
                }
            }
        }

        public static void HandlePlayerMovement(MapleClient client, InPacket iPacket)
        {
            Movements movements = Movements.Decode(iPacket);

            // TODO: Validate movements.

            Movement lastMovement = movements[movements.Count - 1];

            client.Character.Position = lastMovement.Position;
            client.Character.Foothold = lastMovement.Foothold;
            client.Character.Stance = lastMovement.Stance;

            using (OutPacket oPacket = new OutPacket(SendOps.UserMove))
            {
                oPacket.WriteInt(client.Character.ID);
                movements.Encode(oPacket);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }
    }
}
