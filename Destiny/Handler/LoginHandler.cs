using Destiny.Core.IO;
using Destiny.Network;

namespace Destiny.Handler
{
    public static class LoginHandler
    {
        public static void HandleLoginPassword(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadString();
            string password = iPacket.ReadString();
        }
    }
}
