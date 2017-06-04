using Destiny.Network;
using Destiny.Network.Handler;
using Destiny.Utility;

namespace Destiny.Server
{
    public sealed class ShopServer : ServerBase
    {
        public MigrationRegistery Migrations { get; private set; }

        public ShopServer(CShop config)
            : base("Shop", config.Port)
        {
            this.Migrations = new MigrationRegistery();
        }

        public override void Start()
        {
            base.Start();

            Logger.Write(LogLevel.Info, "ShopServer started on port {0}.", this.Port);
        }

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
            client.World = 0;
            client.Channel = 0;
        }
    }
}
