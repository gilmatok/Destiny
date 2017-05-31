using Destiny.Core.IO;

namespace Destiny.Network.Packet
{
    public static class CommonPacket
    {
        public static byte[] Handshake()
        {
            using (OutPacket oPacket = new OutPacket(14, 16))
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteMapleString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                return oPacket.ToArray();
            }
        }
    }
}
