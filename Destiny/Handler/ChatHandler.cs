using Destiny.Core.IO;
using Destiny.Network;
using Destiny.Packet;

namespace Destiny.Handler
{
    public static class ChatHandler
    {
        public static void HandleUserChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith("!"))
            {
                // TODO: Implement a command processing system.
            }
            else
            {
                //client.Character.Map.Broadcast(ChatPacket.UserChat(client.Character.ID, true, text, shout));
            }
        }
    }
}
