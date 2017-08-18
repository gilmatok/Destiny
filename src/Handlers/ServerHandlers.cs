using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Server;
using System;

namespace Destiny.Handlers
{
    public static class ServerHandlers
    {
        public static void HandleChannelMigrate(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();
            iPacket.Skip(2); // NOTE: Unknown.

            if ((accountID = MasterServer.Worlds[client.World][client.Channel].Migrations.Validate(client.Host, characterID)) == -1)
            {
                client.Close();

                return;
            }

            client.Account = new Account(client);
            client.Account.Load(accountID);

            client.Character = new Character(characterID, client);
            client.Character.Load();

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SetField))
            {
                oPacket
                    .WriteInt(client.Channel)
                    .WriteByte(++client.Character.Portals)
                    .WriteBool(true)
                    .WriteShort(); // NOTE: Floating messages at top corner.

                for (int i = 0; i < 3; i++)
                {
                    oPacket.WriteInt(Constants.Random.Next());
                }

                client.Character.EncodeData(oPacket);

                oPacket.WriteDateTime(DateTime.Now);

                client.Send(oPacket);
            }

            client.Character.IsInitialized = true;

            client.Character.Map.Characters.Add(client.Character);

            client.Character.Keymap.Send();

            client.Character.Memos.Send();
        }
    }
}
