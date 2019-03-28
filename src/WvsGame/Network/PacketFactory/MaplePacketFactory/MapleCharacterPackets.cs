using System;
using static Destiny.Constants.CharacterConstants;
using Destiny.Maple.Characters;

namespace Destiny.Network.PacketFactory.MaplePacketFactory
{
    public class MapleCharacterPackets : PacketFactoryManager
    {
        #region GenderPackets
        public static Packet SetGenderPacket(Gender gender)
        {
            Packet setGenderPacket = new Packet(ServerOperationCode.SetGender);
            setGenderPacket.WriteByte((byte) gender);

            return setGenderPacket;
        }

        public static Packet SetGenderPacket(byte gender)
        {
            Packet setGenderPacket = new Packet(ServerOperationCode.SetGender);
            setGenderPacket.WriteByte(gender);

            return setGenderPacket;
        }
        #endregion

        #region  ApperancePackets
        public static Packet UpdateApperancePacket(Character character)
        {
            Packet setApperancePacket = new Packet(ServerOperationCode.AvatarModified);

            setApperancePacket
                    .WriteInt(character.ID)
                    .WriteBool(true)
                    .WriteBytes(character.AppearanceToByteArray())
                    .WriteByte()
                    .WriteShort();

            return setApperancePacket;
        }
        #endregion

        #region  UpdateStatsPackets
        public static Packet UpdateStatsPacket(Character character, params StatisticType[] charStats)
        {
            Packet setStatsPacket = new Packet(ServerOperationCode.StatChanged);

            setStatsPacket.WriteBool(true); // TODO: bOnExclRequest.

            int flag = 0;

            foreach (StatisticType statistic in charStats)
            {
                flag |= (int)statistic;
            }

            setStatsPacket.WriteInt(flag);

            Array.Sort(charStats);

            foreach (StatisticType statistic in charStats)
            {
                switch (statistic)
                {
                    case StatisticType.Skin:
                        setStatsPacket.WriteByte(character.Skin);
                        break;

                    case StatisticType.Face:
                        setStatsPacket.WriteInt(character.Face);
                        break;

                    case StatisticType.Hair:
                        setStatsPacket.WriteInt(character.Hair);
                        break;

                    case StatisticType.Level:
                        setStatsPacket.WriteByte(character.Level);
                        break;

                    case StatisticType.Job:
                        setStatsPacket.WriteShort((short) character.Job);
                        break;

                    case StatisticType.Strength:
                        setStatsPacket.WriteShort(character.Strength);
                        break;

                    case StatisticType.Dexterity:
                        setStatsPacket.WriteShort(character.Dexterity);
                        break;

                    case StatisticType.Intelligence:
                        setStatsPacket.WriteShort(character.Intelligence);
                        break;

                    case StatisticType.Luck:
                        setStatsPacket.WriteShort(character.Luck);
                        break;

                    case StatisticType.Health:
                        setStatsPacket.WriteShort(character.Health);
                        break;

                    case StatisticType.MaxHealth:
                        setStatsPacket.WriteShort(character.MaxHealth);
                        break;

                    case StatisticType.Mana:
                        setStatsPacket.WriteShort(character.Mana);
                        break;

                    case StatisticType.MaxMana:
                        setStatsPacket.WriteShort(character.MaxMana);
                        break;

                    case StatisticType.AbilityPoints:
                        setStatsPacket.WriteShort(character.AbilityPoints);
                        break;

                    case StatisticType.SkillPoints:
                        setStatsPacket.WriteShort(character.SkillPoints);
                        break;

                    case StatisticType.Experience:
                        setStatsPacket.WriteInt(character.Experience);
                        break;

                    case StatisticType.Fame:
                        setStatsPacket.WriteShort(character.Fame);
                        break;

                    case StatisticType.Mesos:
                        setStatsPacket.WriteInt(character.Meso);
                        break;
                }
            }

            return setStatsPacket;
        }
        #endregion

        #region InitializePackets
        public static Packet InitializeCharacterSetFieldPacket(Character character)
        {
            Packet setFieldPacket = new Packet(ServerOperationCode.SetField);

            setFieldPacket
                .WriteInt(WvsGame.ChannelID)
                .WriteByte(++character.Portals)
                .WriteBool(true)
                .WriteShort(); // NOTE: Floating messages at top corner.

            for (int i = 0; i < 3; i++)
            {
                setFieldPacket.WriteInt(Application.Random.Next());
            }

            setFieldPacket
                .WriteBytes(character.DataToByteArray())
                .WriteDateTime(DateTime.UtcNow);

            return setFieldPacket;
        }

        public static Packet InitializeCharacterSrvrStatusChng()
        {
            Packet srvrStatusChng = new Packet(ServerOperationCode.ClaimSvrStatusChanged);

            srvrStatusChng.WriteBool(true);

            return srvrStatusChng;
        }
        #endregion
   
    }
}
