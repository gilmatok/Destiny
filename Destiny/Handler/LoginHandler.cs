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
            foreach(WorldServer world in MasterServer.Instance.Worlds)
            {
                client.Send(LoginPacket.WorldInformation(world));
            }

            client.Send(LoginPacket.WorldEnd());
        }
    }
}
