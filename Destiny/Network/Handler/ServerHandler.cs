using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Game.Characters;
using Destiny.Network.Packet;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Network.Handler
{
    public static class ServerHandler
    {
        public static void HandleMigrateChannel(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Migrations.Validate(client.Host, characterID)) == -1)
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

            client.Character.IsInitialized = true;

            client.Character.Notify(MasterServer.Instance.Worlds[client.World].TickerMessage, NoticeType.Ticker);
        }

        public static void HandleMigrateCashShop(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Instance.Shop.Migrations.Validate(client.Host, characterID)) == -1)
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

            client.Send(ShopPacket.SetCashShop(client.Character));

            client.Character.IsInitialized = true;
        }

        public static void HandleChangeChannel(MapleClient client, InPacket iPacket)
        {
            byte id = iPacket.ReadByte();

            MasterServer.Instance.Worlds[client.World].Channels[id].Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            client.Send(ServerPacket.MigrateCommand(true, MasterServer.Instance.Worlds[client.World].Channels[id].Port));
        }

        public static void HandleCashShop(MapleClient client, InPacket iPacket)
        {
            MasterServer.Instance.Shop.Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            client.Send(ServerPacket.MigrateCommand(true, MasterServer.Instance.Shop.Port));
        }

        public static void HandleMTS(MapleClient client, InPacket iPacket)
        {
            if (client.Character.Map.MapleID == 910000000)
            {
                client.Character.ChangeMap(100000000);
            }
            else
            {
                client.Character.ChangeMap(910000000);
            }
        }
    }
}
