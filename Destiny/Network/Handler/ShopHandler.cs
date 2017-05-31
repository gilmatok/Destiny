using Destiny.Core.IO;
using Destiny.Network.Packet;
using Destiny.Server;

namespace Destiny.Network.Handler
{
    public static class ShopHandler
    {
        public static void OnTransferFieldRequest(MapleClient client, InPacket iPacket)
        {
            MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            client.Send(ServerPacket.MigrateCommand(true, MasterServer.Instance.Worlds[client.World].Channels[client.Channel].Port));
        }
    }
}
