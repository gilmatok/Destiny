using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Handler
{
    public static class ServerHandler
    {
        public static void HandleMigrateChannel(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Channels[client.Channel].Migrations.Validate(client.Host, characterID)) == -1)
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

            client.Character.Initialize();
        }

        public static void HandleMigrateCashShop(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.CashShop.Migrations.Validate(client.Host, characterID)) == -1)
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

            client.Character.Initialize(true);
        }

        public static void HandleChannelChange(MapleClient client, InPacket iPacket)
        {
            byte id = iPacket.ReadByte();

            // TODO: Validate ID.

            MasterServer.Channels[id].Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            using (OutPacket oPacket = new OutPacket(SendOps.MigrateCommand))
            {
                oPacket
                    .WriteBool(true)
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(MasterServer.Channels[id].Port);

                client.Send(oPacket);
            }
        }

        public static void HandleCashShopMigrate(MapleClient client, InPacket iPacket)
        {
            MasterServer.CashShop.Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            using (OutPacket oPacket = new OutPacket(SendOps.MigrateCommand))
            {
                oPacket
                    .WriteBool(true)
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(MasterServer.CashShop.Port);

                client.Send(oPacket);
            }
        }

        public static void HandleMtsMigration(MapleClient client, InPacket iPacket)
        {
            //if (client.Character.Map.MapleID == 910000000)
            //{
            //    client.Character.ChangeMap(100000000);
            //}
            //else
            //{
            //    client.Character.ChangeMap(910000000);
            //}
        }
    }
}