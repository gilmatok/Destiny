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
                    .WriteByte() // NOTE: Portal count.
                    .WriteBool(initial)
                    .WriteShort(); // NOTE: Floating messages at top corner.

                if (initial)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        oPacket.WriteInt(Destiny.Random.Next());
                    }

                    HelpPacket.AddCharacterData(oPacket, character);
                }
                else
                {
                    oPacket
                        .WriteByte()
                        .WriteInt() // NOTE: Map ID.
                        .WriteByte() // NOTE: Map spawn.
                        .WriteShort(character.Health)
                        .WriteBool(false); // NOTE: Follow.
                }

                oPacket.WriteDateTime(DateTime.Now);

                return oPacket.ToArray();
            }
        }
    }
}
