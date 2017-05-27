using Destiny.Core.IO;
using Destiny.Network;

namespace Destiny.Packet
{
    public static class ChatPacket
    {
        public static byte[] UserChat(int characterID, bool isGm, string text, bool shout)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.UserChat))
            {
                oPacket
                    .WriteInt(characterID)
                    .WriteBool(isGm)
                    .WriteString(text)
                    .WriteBool(shout);

                return oPacket.ToArray();
            }
        }
    }
}
