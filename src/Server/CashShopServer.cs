using Destiny.Network;

namespace Destiny.Server
{
    public sealed class CashShopServer : ServerBase
    {
        public MigrationRegistery Migrations { get; private set; }

        public CashShopServer(short port)
            : base("Cash Shop", port)
        {
            this.Migrations = new MigrationRegistery();
        }

        // TODO: We need to get the last world/channel the client was in before the migration
        // to figure out where to send it back after it leaves the Cash Shop. For now, leave it
        // at world 0 channel 0.
        protected override void OnClientAccepted(MapleClient client)
        {
            client.IsInCashShop = true;
            client.Channel = 0;

            base.OnClientAccepted(client);
        }
    }
}
