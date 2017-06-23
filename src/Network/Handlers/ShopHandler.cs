using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Server;

namespace Destiny.Handler
{
    public static class ShopHandler
    {
        public static void OnTransferFieldRequest(MapleClient client, InPacket iPacket)
        {
            MasterServer.Channels[client.Channel].Migrations.Add(client.Host, client.Account.ID, client.Character.ID);

            using (OutPacket oPacket = new OutPacket(SendOps.MigrateCommand))
            {
                oPacket
                    .WriteBool(true)
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(MasterServer.Channels[client.Channel].Port);

                client.Send(oPacket);
            }
        }
    }
}
