using Destiny.Core.IO;
using Destiny.Server;

namespace Destiny.Handler
{
    public static class ShopHandler
    {
        public static void OnTransferFieldRequest(MapleClient client, InPacket iPacket)
        {
            //MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            //client.Migrate(true, MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Port);
        }
    }
}
