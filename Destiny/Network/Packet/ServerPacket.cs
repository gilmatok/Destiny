using Destiny.Core.IO;

namespace Destiny.Network.Packet
{
    public static class ServerPacket
    {
        public static byte[] MigrateCommand(bool valid, short port)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.MigrateCommand))
            {
                oPacket
                    .WriteBool(valid)
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(port);

                return oPacket.ToArray();
            }
        }
    }
}
