using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using System;

namespace Destiny.Packet
{
    public static class MapPacket
    {
        public static byte[] SetField(Character character, bool initial)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.SetField))
            {
                oPacket
                    .WriteInt(character.Client.Channel)
                    .WriteByte(++character.Portals)
                    .WriteBool(initial)
                    .WriteShort(); // NOTE: Floating messages at top corner.

                if (initial)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        oPacket.WriteInt(Destiny.Random.Next());
                    }

                    character.Encode(oPacket);
                }
                else
                {
                    oPacket
                        .WriteByte()
                        .WriteInt(character.Map.MapleID)
                        .WriteByte(character.SpawnPoint)
                        .WriteShort(character.Stats.Health)
                        .WriteBool(false); // NOTE: Follow.
                }

                oPacket.WriteDateTime(DateTime.Now);

                return oPacket.ToArray();
            }
        }
    }
}
