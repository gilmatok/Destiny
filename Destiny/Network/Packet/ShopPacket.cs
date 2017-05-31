using Destiny.Core.IO;
using Destiny.Game.Characters;

namespace Destiny.Network.Packet
{
    public static class ShopPacket
    {
        public static byte[] SetCashShop(Character character)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.SetCashShop))
            {
                character.Encode(oPacket);

                oPacket
                    .WriteByte(1)
                    .WriteMapleString(character.Client.Account.Username)
                    .WriteInt()
                    .WriteShort()
                    .WriteZero(121);

                for (int i = 1; i <= 8; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        oPacket
                            .WriteInt(i)
                            .WriteInt(j)
                            .WriteInt(50200004)
                            .WriteInt(i)
                            .WriteInt(j)
                            .WriteInt(50200069)
                            .WriteInt(i)
                            .WriteInt(j)
                            .WriteInt(50200117)
                            .WriteInt(i)
                            .WriteInt(j)
                            .WriteInt(50100008)
                            .WriteInt(i)
                            .WriteInt(j)
                            .WriteInt(50000047);
                    }
                }

                oPacket
                    .WriteInt()
                    .WriteShort()
                    .WriteByte()
                    .WriteInt(75);

                return oPacket.ToArray();
            }
        }
    }
}
