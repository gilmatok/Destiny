using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network.Packet;
using Destiny.Server;

namespace Destiny.Network.Handler
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
                        string label = iPacket.ReadMapleString();
                        Portal portal = client.Character.Map.Portals[label];

                        if (portal == null)
                        {
                            return;
                        }

                        //Portal destinationPortal = MasterServer.Instance.Data.Maps[portal.DestinationID].Portals[portal.DestinationLabel];

                        //client.Character.ChangeMap(portal.DestinationID, destinationPortal.ID);
                    }
                    break;
            }
        }

        public static void OnChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
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

        public static void OnMove(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(9);
            int rewindOffset = iPacket.Position;
            client.Character.Map.DecodeMovePath(client.Character, iPacket);
            iPacket.Position = rewindOffset;

            client.Character.Map.Broadcast(UserPacket.UserMove(client.Character.ID, iPacket.ReadBytes(iPacket.Remaining)), client.Character);
        }
    }
}
