using Destiny.Core.IO;
using Destiny.Game;

namespace Destiny.Packet
{
    public static class HelpPacket
    {
        public static void AddCharacterEntry(OutPacket oPacket, Character character)
        {
            HelpPacket.AddCharacterStatistics(oPacket, character);
            HelpPacket.AddCharacterAppearance(oPacket, character);

            oPacket
                .WriteByte()
                .WriteBool();
        }

        public static void AddCharacterStatistics(OutPacket oPacket, Character character)
        {
            oPacket
                .WriteInt(character.ID)
                .WriteStringFixed(character.Name, 13)
                .WriteByte((byte)character.Gender)
                .WriteByte(character.Skin)
                .WriteInt(character.Face)
                .WriteInt(character.Hair)
                .WriteLong()
                .WriteLong()
                .WriteLong()
                .WriteByte(character.Level)
                .WriteShort((short)character.Job)
                .WriteShort(character.Strength)
                .WriteShort(character.Dexterity)
                .WriteShort(character.Intelligence)
                .WriteShort(character.Luck)
                .WriteShort(character.Health)
                .WriteShort(character.MaxHealth)
                .WriteShort(character.Mana)
                .WriteShort(character.MaxMana)
                .WriteShort(character.AbilityPoints)
                .WriteShort(character.SkillPoints)
                .WriteInt(character.Experience)
                .WriteShort(character.Fame)
                .WriteInt()
                .WriteInt()
                .WriteByte()
                .WriteInt();
        }

        public static void AddCharacterAppearance(OutPacket oPacket, Character character)
        {
            oPacket
                .WriteByte((byte)character.Gender)
                .WriteByte(character.Skin)
                .WriteInt(character.Face)
                .WriteBool(true)
                .WriteInt(character.Hair)
                .WriteByte(byte.MaxValue)
                .WriteByte(byte.MaxValue)
                .WriteInt()
                .WriteZero(12);
        }
    }
}
