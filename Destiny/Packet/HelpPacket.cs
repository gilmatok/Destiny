using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Destiny.Packet
{
    public static class HelpPacket
    {
        public static void AddCharacterEntry(OutPacket oPacket, DatabaseQuery query)
        {
            oPacket
                .WriteInt(query.GetInt("character_id"))
                .WriteStringFixed(query.GetString("name"), 13)
                .WriteByte(query.GetByte("gender"))
                .WriteByte(query.GetByte("skin"))
                .WriteInt(query.GetInt("face"))
                .WriteInt(query.GetInt("hair"))
                .WriteLong()
                .WriteLong()
                .WriteLong()
                .WriteByte(query.GetByte("level"))
                .WriteShort(query.GetShort("job"))
                .WriteShort(query.GetShort("strength"))
                .WriteShort(query.GetShort("dexterity"))
                .WriteShort(query.GetShort("intelligence"))
                .WriteShort(query.GetShort("luck"))
                .WriteShort(query.GetShort("health"))
                .WriteShort(query.GetShort("max_health"))
                .WriteShort(query.GetShort("mana"))
                .WriteShort(query.GetShort("max_mana"))
                .WriteShort(query.GetShort("ability_points"))
                .WriteShort(query.GetShort("skill_points"))
                .WriteInt(query.GetInt("experience"))
                .WriteShort(query.GetShort("fame"))
                .WriteInt()
                .WriteInt(query.GetInt("map"))
                .WriteByte(query.GetByte("map_spawn"))
                .WriteInt();

            oPacket
                .WriteByte(query.GetByte("gender"))
                .WriteByte(query.GetByte("skin"))
                .WriteInt(query.GetInt("face"))
                .WriteBool(true)
                .WriteInt(query.GetInt("hair"));

            SortedDictionary<byte, Doublet<int, int>> equipment = new SortedDictionary<byte, Doublet<int, int>>();

            using (DatabaseQuery equipmentQuery = Database.Query("SELECT `slot`, `item_identifier` FROM `items` WHERE `character_id` = @character_id AND `inventory` = 0 AND `slot` < 0", new MySqlParameter("@character_id", query.GetInt("character_id"))))
            {
                while (equipmentQuery.NextRow())
                {
                    short slot = (short)(-(equipmentQuery.GetShort("slot")));

                    if (slot > 100)
                    {
                        slot -= 100;
                    }

                    Doublet<int, int> pair = equipment.GetOrDefault((byte)slot, null);

                    if (pair == null)
                    {
                        pair = new Doublet<int, int>(equipmentQuery.GetInt("item_identifier"), 0);
                        equipment.Add((byte)slot, pair);
                    }
                    else if (equipmentQuery.GetShort("slot") < -100)
                    {
                        pair.Second = pair.First;
                        pair.First = equipmentQuery.GetInt("item_identifier");
                    }
                    else
                    {
                        pair.Second = (int)equipmentQuery["item_identifier"];
                    }
                }
            }

            foreach (KeyValuePair<byte, Doublet<int, int>> pair in equipment)
            {
                oPacket.WriteByte(pair.Key);

                if (pair.Key == 11 && pair.Value.Second > 0)
                {
                    oPacket.WriteInt(pair.Value.Second);
                }
                else
                {
                    oPacket.WriteInt(pair.Value.First);
                }
            }
            oPacket.WriteByte(byte.MaxValue);

            foreach (KeyValuePair<byte, Doublet<int, int>> pair in equipment)
            {
                if (pair.Key != 11 && pair.Value.Second > 0)
                {
                    oPacket
                        .WriteByte(pair.Key)
                        .WriteInt(pair.Value.Second);
                }
            }
            oPacket.WriteByte(byte.MaxValue);

            Doublet<int, int> cashWeapon = equipment.GetOrDefault((byte)11, null);

            oPacket
                .WriteInt(cashWeapon == null ? 0 : cashWeapon.First)
                .WriteZero(12)
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

        public static void AddCharacterData(OutPacket oPacket, Character character, long flag = long.MaxValue)
        {
            oPacket
                .WriteLong(flag)
                .WriteByte(); // NOTE: Unknown.

            HelpPacket.AddCharacterStatistics(oPacket, character);

            oPacket
                .WriteByte(20) // NOTE: Max buddylist size.
                .WriteBool(false) // NOTE: Blessing of Fairy.
                .WriteInt(); // NOTE: Mesos.

            character.Items.Encode(oPacket);
            character.Skills.Encode(oPacket);
            character.Quests.Encode(oPacket);

            oPacket
                .WriteShort() // NOTE: Mini games record.
                .WriteShort() // NOTE: Rings (1).
                .WriteShort() // NOTE: Rings (2). 
                .WriteShort(); // NOTE: Rings (3).

            // NOTE: Teleport rock locations.
            for (int i = 0; i < 15; i++)
            {
                oPacket.WriteInt(Map.INVALID_MAP_ID);
            }

            oPacket
                .WriteInt() // NOTE: Monster book cover ID.
                .WriteByte() // NOTE: Unknown.
                .WriteShort() // NOTE: Monster book cards count.
                .WriteShort() // NOTE: New year cards.
                .WriteShort() // NOTE: Area information.
                .WriteShort(); // NOTE: Unknown.
        }
    }
}
