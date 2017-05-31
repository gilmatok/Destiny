using Destiny.Core.IO;
using Destiny.Core.Security;
using Destiny.Game;
using Destiny.Network;
using Destiny.Network.Packet;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Network.Handler
{
    public static class LoginHandler
    {
        // TODO: Handle different scenarios (ban, quiet ban, etcetera).
        public static void HandleLoginPassword(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadMapleString();
            string password = iPacket.ReadMapleString();

            Account account;

            using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `username` = @username", new MySqlParameter("username", username)))
            {
                if (!query.NextRow())
                {
                    if (MasterServer.Instance.Login.AutoRegister && username == client.LastUsername && password == client.LastPassword)
                    {
                        // TODO: Auto register.
                    }
                    else
                    {
                        client.Send(LoginPacket.LoginError(LoginResult.NotRegistered));
                    }

                    return;
                }

                account = new Account(query);
            }

            if (SHACryptograph.Encrypt(SHAMode.SHA512, password + account.Salt) != account.Password)
            {
                client.Send(LoginPacket.LoginError(LoginResult.IncorrectPassword));
            }
            else
            {
                client.Account = account;

                client.Send(LoginPacket.LoginSuccess(account));
            }
        }

        public static void HandleWorldList(MapleClient client, InPacket iPacket)
        {
            foreach (WorldServer world in MasterServer.Instance.Worlds)
            {
                client.Send(LoginPacket.WorldInformation(world));
            }

            client.Send(LoginPacket.WorldEnd());
        }

        public static void HandleCheckUserLimit(MapleClient client, InPacket iPacket)
        {
            byte worldID = iPacket.ReadByte();
            WorldServer world = MasterServer.Instance.Worlds[worldID];

            client.Send(LoginPacket.CheckUserLimitResult(world.Status));
        }


        public static void HandleWorldSELECT(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1);
            client.World = iPacket.ReadByte();
            client.Channel = iPacket.ReadByte();

            client.Send(LoginPacket.SelectWorldResult(client.Account.ID, client.World));
        }

        public static void HandleCharacterNameCheck(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = (long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `name` = @name", new MySqlParameter("name", name)) != 0;

            client.Send(LoginPacket.CheckDuplicatedIDResult(name, unusable));
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

            // TODO: Validate name, beauty and equipment before creating the character.

            int id = Database.InsertAndReturnIdentifier("INSERT INTO `characters` (account_id, world_id, name, gender, skin, face, hair) " +
                                                        "VALUES (@account_id, @world_id, @name, @gender, @skin, @face, @hair)",
                                                        new MySqlParameter("account_id", client.Account.ID),
                                                        new MySqlParameter("world_id", client.World),
                                                        new MySqlParameter("name", name),
                                                        new MySqlParameter("gender", gender),
                                                        new MySqlParameter("skin", skin),
                                                        new MySqlParameter("face", face),
                                                        new MySqlParameter("hair", hair));

            // TODO: Validate the default equipment statistics. I'm pretty sure some of them are untradable.

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, item_identifier, weapon_defense) " +
                             "VALUES (@character_id, 0, -5, @item_identifier, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("item_identifier", topID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, item_identifier, weapon_defense) " +
                             "VALUES (@character_id, 0, -6, @item_identifier, 2)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("item_identifier", bottomID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, item_identifier, slots, weapon_defense) " +
                             "VALUES (@character_id, 0, -7, @item_identifier, 5, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("item_identifier", shoesID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, item_identifier, slots, weapon_attack) " +
                             "VALUES (@character_id, 0, -11, @item_identifier, 7, 17)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("item_identifier", weaponID));

            // TODO: Add beginner's guide (based on job).

            using (DatabaseQuery query = Database.Query("SELECT * FROM characters WHERE `character_id` = @character_id", new MySqlParameter("character_id", id)))
            {
                query.NextRow();

                client.Send(LoginPacket.CreateNewCharacterResult(false, query));
            }
        }

        public static void HandleCharacterSelection(MapleClient client, InPacket iPacket)
        {
            int characterID = iPacket.ReadInt();
            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            LoginHandler.MigrateClient(client, characterID);
        }

        private static void MigrateClient(MapleClient client, int characterID)
        {
            MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Migrations.Add(client.Host, client.Account.ID, characterID);

            client.Send(LoginPacket.SelectCharacterResult(MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Port, characterID));
        }
    }
}
