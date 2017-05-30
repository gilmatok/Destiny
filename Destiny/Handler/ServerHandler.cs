using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using Destiny.Packet;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Handler
{
    public static class ServerHandler
    {
        public static void OnMigrateIn(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Instance.Worlds[client.World].EligableMigration(client.Host, characterID)) == 0)
            {
                client.Close();

                return;
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `account_id` = @account_id", new MySqlParameter("account_id", accountID)))
            {
                query.NextRow();

                client.Account = new Account(query);
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `character_id` = @character_id", new MySqlParameter("character_id", characterID)))
            {
                query.NextRow();

                client.Character = new Character(client, query);
            }

            client.Send(MapPacket.SetField(client.Character, true));

            client.Character.Map.Characters.Add(client.Character);

            client.Character.Notify(MasterServer.Instance.Worlds[client.World].TickerMessage, NoticeType.Ticker);

            client.Character.IsInitialized = true;
        }

        public static void HandleMigrateCashShop(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Instance.Worlds[client.World].EligableMigration(client.Host, characterID)) == 0)
            {
                client.Close();

                return;
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `account_id` = @account_id", new MySqlParameter("account_id", accountID)))
            {
                query.NextRow();

                client.Account = new Account(query);
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `character_id` = @character_id", new MySqlParameter("character_id", characterID)))
            {
                query.NextRow();

                client.Character = new Character(client, query);
            }

            // TODO: Cash Shop packets and whatnot.
        }
    }
}
