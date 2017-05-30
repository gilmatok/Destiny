using Destiny.Core.IO;
using Destiny.Network;
using Destiny.Packet;
using Destiny.Server;

namespace Destiny.Handler
{
    public static class UserHandler
    {
        public static void HandleUserChat(MapleClient client, InPacket iPacket)
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
