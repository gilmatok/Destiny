using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Core.Security;
using Destiny.Maple;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Destiny.Handler
{
    public static class LoginHandler
    {
        public static void HandleLoginPassword(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadMapleString();
            string password = iPacket.ReadMapleString();

            LoginResult result = LoginResult.Valid;

            if (!username.IsAlphaNumeric())
            {
                result = LoginResult.InvalidUsername;
            }
            else
            {
                Account account = null;

                using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `username` = @username", new MySqlParameter("username", username)))
                {
                    if (query.NextRow())
                    {
                        account = new Account(query);
                    }
                }

                if (account == null)
                {
                    result = LoginResult.InvalidUsername;
                }
                else if (SHACryptograph.Encrypt(SHAMode.SHA512, password + account.Salt) != account.Password)
                {
                    result = LoginResult.InvalidPassword;
                }
                else if (false) // TODO: Handle ban scenario.
                {
                    result = LoginResult.Banned;
                }
                else if (false) // TODO: Handle logged-in scenario.
                {
                    result = LoginResult.LoggedIn;
                }
                else
                {
                    client.Account = account;
                }
            }

            using (OutPacket oPacket = new OutPacket(SendOps.CheckPasswordResult))
            {
                oPacket
                    .WriteInt((int)result)
                    .WriteByte()
                    .WriteByte();

                if (result == LoginResult.Valid)
                {
                    oPacket
                        .WriteInt(client.Account.ID)
                        .WriteByte()
                        .WriteBool()
                        .WriteByte()
                        .WriteByte()
                        .WriteMapleString(client.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteShort(2);
                }

                client.Send(oPacket);
            }
        }

        public static void HandleWorldList(MapleClient client, InPacket iPacket)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.WorldInformation))
            {
                oPacket
                    .WriteByte()
                    .WriteMapleString("Destiny")
                    .WriteByte((byte)WorldFlag.New)
                    .WriteMapleString("Welcome to #rDestiny#k!")
                    .WriteShort(100)
                    .WriteShort(100)
                    .WriteByte()
                    .WriteByte((byte)MasterServer.Channels.Length);

                foreach (ChannelServer channel in MasterServer.Channels)
                {
                    oPacket
                        .WriteMapleString(channel.Label)
                        .WriteInt(channel.Load)
                        .WriteByte(1)
                        .WriteShort(channel.ID);
                }

                oPacket.WriteShort();

                client.Send(oPacket);
            }

            using (OutPacket oPacket = new OutPacket(SendOps.WorldInformation))
            {
                oPacket.WriteByte(byte.MaxValue);

                client.Send(oPacket);
            }
        }

        public static void HandleCheckUserLimit(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1); // NOTE: World ID, but we're not using it.

            using (OutPacket oPacket = new OutPacket(SendOps.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)WorldStatus.Normal);

                client.Send(oPacket);
            }
        }

        public static void HandleWorldSelect(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1);
            iPacket.Skip(1); // NOTE: World ID, but we're not using it.
            client.Channel = iPacket.ReadByte();

            byte characterCount = (byte)(long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `account_id` = @account_id", new MySqlParameter("@account_id", client.Account.ID));

            using (OutPacket oPacket = new OutPacket(SendOps.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte(characterCount);

                if (characterCount > 0)
                {
                    using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `account_id` = @account_id", new MySqlParameter("@account_id", client.Account.ID)))
                    {
                        while (query.NextRow())
                        {
                            LoginHandler.AddCharacterEntry(oPacket, query);
                        }
                    }
                }

                oPacket
                    .WriteByte(2)
                    .WriteInt(3); // TODO: Account specific character creation slots. For now, use default 3.

                client.Send(oPacket);
            }
        }

        private static void AddCharacterEntry(OutPacket oPacket, DatabaseQuery query)
        {
            oPacket
                .WriteInt(query.GetInt("character_id"))
                .WritePaddedString(query.GetString("name"), 13)
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
                .WriteByte(query.GetByte("spawn_point"))
                .WriteInt();

            oPacket
                .WriteByte(query.GetByte("gender"))
                .WriteByte(query.GetByte("skin"))
                .WriteInt(query.GetInt("face"))
                .WriteBool(true)
                .WriteInt(query.GetInt("hair"));

            Dictionary<byte, int> visibleLayer = new Dictionary<byte, int>();
            Dictionary<byte, int> hiddenLayer = new Dictionary<byte, int>();

            using (DatabaseQuery equipmentQuery = Database.Query("SELECT `slot`, `maple_id` FROM `items` WHERE `character_id` = @character_id AND `inventory` = 1 AND `slot` < 0", new MySqlParameter("@character_id", query.GetInt("character_id"))))
            {
                while (equipmentQuery.NextRow())
                {
                    byte slot = (byte)(short)(-(equipmentQuery.GetShort("slot")));
                    int mapleID = equipmentQuery.GetInt("maple_id");

                    if (slot < 100 && !visibleLayer.ContainsKey(slot))
                    {
                        visibleLayer[slot] = mapleID;
                    }
                    else if (slot > 100 && slot != 111)
                    {
                        slot -= 100;

                        if (visibleLayer.ContainsKey(slot))
                        {
                            hiddenLayer[slot] = visibleLayer[slot];
                        }

                        visibleLayer[slot] = mapleID;
                    }
                    else if (visibleLayer.ContainsKey(slot))
                    {
                        hiddenLayer[slot] = mapleID;
                    }
                }
            }

            foreach (KeyValuePair<byte, int> entry in visibleLayer)
            {
                oPacket
                    .WriteByte(entry.Key)
                    .WriteInt(entry.Value);
            }

            oPacket.WriteByte(byte.MaxValue);

            foreach (KeyValuePair<byte, int> entry in hiddenLayer)
            {
                oPacket
                    .WriteByte(entry.Key)
                    .WriteInt(entry.Value);
            }

            oPacket.WriteByte(byte.MaxValue);

            oPacket
                .WriteInt() // TODO: Cash weapon.
                .WriteZero(12)
                .WriteByte()
                .WriteBool();
        }

        public static void HandleCharacterNameCheck(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = (long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `name` = @name", new MySqlParameter("name", name)) != 0;

            using (OutPacket oPacket = new OutPacket(SendOps.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteMapleString(name)
                    .WriteBool(unusable);

                client.Send(oPacket);
            }
        }

        public static void HandleCharacterCreation(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            int jobType = iPacket.ReadInt();
            int face = iPacket.ReadInt();
            int hair = iPacket.ReadInt();
            int hairColor = iPacket.ReadInt();
            byte skin = (byte)iPacket.ReadInt();
            int topID = iPacket.ReadInt();
            int bottomID = iPacket.ReadInt();
            int shoesID = iPacket.ReadInt();
            int weaponID = iPacket.ReadInt();
            Gender gender = (Gender)iPacket.ReadByte();

            bool error = false;

            // TODO: Validate name, beauty and equipment before creating the character.

            int id = Database.InsertAndReturnIdentifier("INSERT INTO `characters` (account_id, name, gender, skin, face, hair) " +
                                                        "VALUES (@account_id, @name, @gender, @skin, @face, @hair)",
                                                        new MySqlParameter("account_id", client.Account.ID),
                                                        new MySqlParameter("name", name),
                                                        new MySqlParameter("gender", gender),
                                                        new MySqlParameter("skin", skin),
                                                        new MySqlParameter("face", face),
                                                        new MySqlParameter("hair", hair));

            // TODO: Validate the default equipment statistics. I'm pretty sure some of them are untradable.

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, weapon_defense) " +
                             "VALUES (@character_id, 1, -5, @maple_id, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", topID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, weapon_defense) " +
                             "VALUES (@character_id, 1, -6, @maple_id, 2)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", bottomID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, slots, weapon_defense) " +
                             "VALUES (@character_id, 1, -7, @maple_id, 5, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", shoesID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, slots, weapon_attack) " +
                             "VALUES (@character_id, 1, -11, @maple_id, 7, 17)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", weaponID));

            // TODO: Add beginner's guide (based on job).

            using (DatabaseQuery query = Database.Query("SELECT * FROM characters WHERE `character_id` = @character_id", new MySqlParameter("character_id", id)))
            {
                query.NextRow();

                using (OutPacket oPacket = new OutPacket(SendOps.CreateNewCharacterResult))
                {
                    oPacket.WriteBool(error);

                    LoginHandler.AddCharacterEntry(oPacket, query);

                    client.Send(oPacket);
                }
            }
        }

        public static void HandleCharacterSelection(MapleClient client, InPacket iPacket)
        {
            int characterID = iPacket.ReadInt();
            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            MasterServer.Channels[client.Channel].Migrations.Add(client.Host, client.Account.ID, characterID);

            using (OutPacket oPacket = new OutPacket(SendOps.SelectCharacterResult))
            {
                oPacket
                    .WriteByte()
                    .WriteByte()
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(MasterServer.Channels[client.Channel].Port)
                    .WriteInt(characterID)
                    .WriteInt()
                    .WriteByte();

                client.Send(oPacket);
            }
        }
    }
}