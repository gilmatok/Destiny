using Destiny.Core.IO;
using Destiny.Network;
using System;
using Destiny.Game.Characters;

namespace Destiny.Network.Packet
{
    public static class MapPacket
    {
        public static byte[] SetField(Character character, bool initial)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.SetField))
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
                        oPacket.WriteInt(Constants.Random.Next());
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
