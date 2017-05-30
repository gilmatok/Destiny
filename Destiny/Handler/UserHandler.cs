using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network;
using Destiny.Packet;
using Destiny.Server;

namespace Destiny.Handler
{
    public static class UserHandler
    {
        public static void OnTransferFieldRequest(MapleClient client, InPacket iPacket)
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
                        string label = iPacket.ReadString();
                        Portal portal = client.Character.Map.Portals[label];

                        if (portal == null)
                        {
                            return;
                        }

                        Portal destinationPortal = MasterServer.Instance.Data.Maps[portal.DestinationID].Portals[portal.DestinationLabel];

                        client.Character.ChangeMap(portal.DestinationID, destinationPortal.ID);
                    }
                    break;
            }
        }

        public static void OnChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                MasterServer.Instance.Commands.Execute(client.Character, text);
            }
            else
            {
                client.Character.Map.Broadcast(UserPacket.UserChat(client.Character.ID, client.Character.IsGm, text, shout));
            }
        }
    }
}
