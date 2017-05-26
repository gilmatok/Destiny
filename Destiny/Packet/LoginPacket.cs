using Destiny.Core.IO;

namespace Destiny.Packet
{
    public static class LoginPacket
    {
        public static byte[] Handshake()
        {
            using (OutPacket oPacket = new OutPacket(14, 16))
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                return oPacket.ToArray();
            }
        }
    }
}
