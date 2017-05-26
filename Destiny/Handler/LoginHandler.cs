using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using Destiny.Packet;
using Destiny.Server;

namespace Destiny.Handler
{
    public static class LoginHandler
    {
        // TODO: Handle different scenarios (ban, quiet ban, etcetera).
        public static void HandleLoginPassword(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadString();
            string password = iPacket.ReadString();

            Account account = MasterServer.Instance.Database.GetAccount(username);

            if (account == null)
            {
                // TODO: Check if login configuration allows auto registering. For now, enable it.
                if (true && username == client.LastUsername && password == client.LastPassword)
                {
                    account = new Account
                    {
                        Username = username,
                        Password = password
                    };

                    MasterServer.Instance.Database.AddAccount(account);

                    client.Account = account;

                    client.Send(LoginPacket.LoginSuccess(account));
                }
                else
                {
                    client.Send(LoginPacket.LoginError(LoginResult.NotRegistered));

                    client.LastUsername = username;
                    client.LastPassword = password;
                }
            }
            else if (password != account.Password)
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

            var characters = MasterServer.Instance.Database.GetCharacters(client.Account.AccountId, client.World);

            client.Send(LoginPacket.SelectWorldResult(characters));
        }

        public static void HandleCheckCharacterName(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadString();
            bool taken = MasterServer.Instance.Database.CharacterNameTaken(name);

            client.Send(LoginPacket.CheckDuplicatedIDResult(name, taken));
        }

        public static void HandleCreateCharacter(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadString();
            int jobType = iPacket.ReadInt();
            int face = iPacket.ReadInt();
            int hair = iPacket.ReadInt();
            int hairColor = iPacket.ReadInt();
            byte skin = (byte) iPacket.ReadInt();
            int topID = iPacket.ReadInt();
            int bottomID = iPacket.ReadInt();
            int shoesID = iPacket.ReadInt();
            int weaponID = iPacket.ReadInt();
            Gender gender = (Gender)iPacket.ReadByte();

            Character character = new Character
            {
                AccountId = client.Account.AccountId,
                WorldId = client.World,
                Name = name,
                Gender = gender,
                Skin = skin,
                Face = face,
                Hair = hair + hairColor,
                Level = 1,
                Job = Job.Beginner,
                Strength = 12,
                Dexterity = 5,
                Intelligence = 4,
                Luck = 4,
                Health = 50,
                MaxHealth = 50,
                Mana = 5,
                MaxMana = 5,
                AbilityPoints = 0,
                SkillPoints = 0,
                Experience = 0,
                Fame = 0
            };

            MasterServer.Instance.Database.AddCharacter(character);

            client.Send(LoginPacket.CreateNewCharacterResult(false, character));
        }
    }
}
