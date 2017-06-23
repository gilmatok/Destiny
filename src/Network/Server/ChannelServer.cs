using Destiny.Core.Network;
using Destiny.Maple.Maps;
using Destiny.Handler;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public MapFactory Maps { get; private set; }

        public ChannelServer(byte id, short port)
            : base(string.Format("Destiny-{0}", id), port)
        {
            this.ID = id;
            this.Maps = new MapFactory(this.ID);
        }

        public override void Start()
        {
            base.Start();
        }

        protected override void SpawnHandlers()
        {
            this.AddHandler(RecvOps.MigrateIn, ServerHandler.HandleMigrateChannel);
            this.AddHandler(RecvOps.MapChange, PlayerHandler.HandleMapChange);
            this.AddHandler(RecvOps.ChannelChange, ServerHandler.HandleChannelChange);
            this.AddHandler(RecvOps.CashShopMigration, ServerHandler.HandleCashShopMigrate);
            this.AddHandler(RecvOps.MtsMigration, ServerHandler.HandleMtsMigration);
            this.AddHandler(RecvOps.PlayerChat, PlayerHandler.HandlePlayerChat);
            this.AddHandler(RecvOps.PlayerMovement, PlayerHandler.HandlePlayerMovement);
            this.AddHandler(RecvOps.ItemMovement, InventoryHandler.HandleItemMovement);
            this.AddHandler(RecvOps.NpcMovement, NpcHandler.HandleNpcMovement);
            this.AddHandler(RecvOps.MobMovement, MobHandler.HandleMobMovement);
            this.AddHandler(RecvOps.PlayerInformation, PlayerHandler.HandlePlayerInformation);
        }

        protected override void SetClientAttributes(MapleClient client)
        {
            client.Channel = this.ID;
        }
    }
}
