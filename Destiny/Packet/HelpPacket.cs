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
    }
}
