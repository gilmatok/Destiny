using System;
using Destiny.Constants;
using Destiny.Maple.Characters;

namespace Destiny.Network.PacketFactory.MaplePacketFactory
{
    public class MapleCharacterPackets : PacketFactoryManager
    {
        #region GenderPackets
        public static Packet SetGenderPacket(CharacterConstants.Gender gender)
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
        public static Packet UpdateStatsPacket(Character character, params CharacterConstants.StatisticType[] charStats)
        {
            Packet setStatsPacket = new Packet(ServerOperationCode.StatChanged);

            setStatsPacket.WriteBool(true); // TODO: bOnExclRequest.

            int flag = 0;

            foreach (CharacterConstants.StatisticType statistic in charStats)
            {
                flag |= (int)statistic;
            }

            setStatsPacket.WriteInt(flag);

            Array.Sort(charStats);

            foreach (CharacterConstants.StatisticType statistic in charStats)
            {
                switch (statistic)
                {
                    case CharacterConstants.StatisticType.Skin:
                        setStatsPacket.WriteByte(character.Skin);
                        break;

                    case CharacterConstants.StatisticType.Face:
                        setStatsPacket.WriteInt(character.Face);
                        break;

                    case CharacterConstants.StatisticType.Hair:
                        setStatsPacket.WriteInt(character.Hair);
                        break;

                    case CharacterConstants.StatisticType.Level:
                        setStatsPacket.WriteByte(character.Level);
                        break;

                    case CharacterConstants.StatisticType.Job:
                        setStatsPacket.WriteShort((short) character.Job);
                        break;

                    case CharacterConstants.StatisticType.Strength:
                        setStatsPacket.WriteShort(character.Strength);
                        break;

                    case CharacterConstants.StatisticType.Dexterity:
                        setStatsPacket.WriteShort(character.Dexterity);
                        break;

                    case CharacterConstants.StatisticType.Intelligence:
                        setStatsPacket.WriteShort(character.Intelligence);
                        break;

                    case CharacterConstants.StatisticType.Luck:
                        setStatsPacket.WriteShort(character.Luck);
                        break;

                    case CharacterConstants.StatisticType.Health:
                        setStatsPacket.WriteShort(character.Health);
                        break;

                    case CharacterConstants.StatisticType.MaxHealth:
                        setStatsPacket.WriteShort(character.MaxHealth);
                        break;

                    case CharacterConstants.StatisticType.Mana:
                        setStatsPacket.WriteShort(character.Mana);
                        break;

                    case CharacterConstants.StatisticType.MaxMana:
                        setStatsPacket.WriteShort(character.MaxMana);
                        break;

                    case CharacterConstants.StatisticType.AbilityPoints:
                        setStatsPacket.WriteShort(character.AbilityPoints);
                        break;

                    case CharacterConstants.StatisticType.SkillPoints:
                        setStatsPacket.WriteShort(character.SkillPoints);
                        break;

                    case CharacterConstants.StatisticType.Experience:
                        setStatsPacket.WriteInt(character.Experience);
                        break;

                    case CharacterConstants.StatisticType.Fame:
                        setStatsPacket.WriteShort(character.Fame);
                        break;

                    case CharacterConstants.StatisticType.Mesos:
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
