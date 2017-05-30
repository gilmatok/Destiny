using Destiny.Core.IO;
using Destiny.Game.Characters;
using Destiny.Network;
using System;

namespace Destiny.Packet
{
    public static class UserPacket
    {
        public static byte[] StatChanged(Character character, params StatisticType[] statistics)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.StatChanged))
            {
                oPacket.WriteBool(); // TODO: bOnExclRequest.

                int flag = 0;

                foreach (StatisticType statistic in statistics)
                {
                    flag |= (int)statistic;
                }

                oPacket.WriteInt(flag);

                Array.Sort(statistics);

                foreach (StatisticType statistic in statistics)
                {
                    switch (statistic)
                    {
                        case StatisticType.Skin:
                            oPacket.WriteByte(character.Stats.Skin);
                            break;

                        case StatisticType.Face:
                            oPacket.WriteInt(character.Stats.Face);
                            break;

                        case StatisticType.Hair:
                            oPacket.WriteInt(character.Stats.Hair);
                            break;

                        case StatisticType.Level:
                            oPacket.WriteByte(character.Stats.Level);
                            break;

                        case StatisticType.Job:
                            oPacket.WriteShort((short)character.Stats.Job);
                            break;

                        case StatisticType.Strength:
                            oPacket.WriteShort(character.Stats.Strength);
                            break;

                        case StatisticType.Dexterity:
                            oPacket.WriteShort(character.Stats.Dexterity);
                            break;

                        case StatisticType.Intelligence:
                            oPacket.WriteShort(character.Stats.Intelligence);
                            break;

                        case StatisticType.Luck:
                            oPacket.WriteShort(character.Stats.Luck);
                            break;

                        case StatisticType.Health:
                            oPacket.WriteShort(character.Stats.Health);
                            break;

                        case StatisticType.MaxHealth:
                            oPacket.WriteShort(character.Stats.MaxHealth);
                            break;

                        case StatisticType.Mana:
                            oPacket.WriteShort(character.Stats.Mana);
                            break;

                        case StatisticType.MaxMana:
                            oPacket.WriteShort(character.Stats.MaxMana);
                            break;

                        case StatisticType.AbilityPoints:
                            oPacket.WriteShort(character.Stats.AbilityPoints);
                            break;

                        case StatisticType.SkillPoints:
                            oPacket.WriteShort(character.Stats.SkillPoints);
                            break;

                        case StatisticType.Experience:
                            oPacket.WriteInt(character.Stats.Experience);
                            break;

                        case StatisticType.Fame:
                            oPacket.WriteShort(character.Stats.Fame);
                            break;

                        case StatisticType.Mesos:
                            oPacket.WriteInt(character.Stats.Mesos);
                            break;
                    }
                }

                return oPacket.ToArray();
            }
        }

        public static byte[] BrodcastMsg(string message, NoticeType type)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.BroadcastMsg))
            {
                oPacket.WriteByte((byte)type);

                if (type == NoticeType.Ticker)
                {
                    oPacket.WriteBool(!string.IsNullOrEmpty(message));
                }

                oPacket.WriteString(message);

                return oPacket.ToArray();
            }
        }

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
