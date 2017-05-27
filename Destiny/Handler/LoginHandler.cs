using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using Destiny.Packet;
using Destiny.Security;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Handler
{
    public static class LoginHandler
    {
        // TODO: Handle different scenarios (ban, quiet ban, etcetera).
        public static void HandleLoginPassword(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadString();
            string password = iPacket.ReadString();

            Account account;

            using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `username` = @username", new MySqlParameter("username", username)))
            {
                if (!query.Read())
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

            if (SHACryptograph.Encrypt(SHAMode.SHA512, password + account.PasswordSalt) != account.Password)
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


        public static void HandleSelectWorld(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1);
            client.World = iPacket.ReadByte();
            client.Channel = iPacket.ReadByte();

            //client.Send(LoginPacket.SelectWorldResult(characters));
        }

        public static void HandleCheckCharacterName(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadString();
            bool unusable = (long)Database.Scalar("SELECT COUNT(*) FROM `character`s WHERE `name` = @name", new MySqlParameter("name", name)) != 0;

            client.Send(LoginPacket.CheckDuplicatedIDResult(name, unusable));
        }

        public static void HandleCreateCharacter(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadString();
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
        }
    }
}
