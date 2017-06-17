using Destiny.Core.Network;
using Destiny.Handler;

namespace Destiny.Server
{
    public sealed class CashShopServer : ServerBase
    {
        public CashShopServer(short port) : base("Cash Shop", port) { }

        protected override void SpawnHandlers()
        {
            this.AddHandler(RecvOps.MigrateIn, ServerHandler.HandleMigrateCashShop);
            this.AddHandler(RecvOps.MapChange, ShopHandler.OnTransferFieldRequest);
        }

        // TODO: We need to get the last world/channel the client was in before the migration
        // to figure out where to send it back after it leaves the Cash Shop. For now, leave it
        // at world 0 channel 0.
        protected override void SetClientAttributes(MapleClient client)
        {
            client.Channel = 0;
        }
    }
}
