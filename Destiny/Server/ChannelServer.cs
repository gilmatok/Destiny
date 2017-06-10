using Destiny.Core.Network;
using Destiny.Game.Data;
using Destiny.Game.Maps;
using Destiny.Handler;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class ChannelMaps : Dictionary<int, Map>
    {
        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public new Map this[int mapleID]
        {
            get
            {
                if (!base.ContainsKey(mapleID))
                {
                    Map map = new Map(mapleID, this.World, this.Channel);

                    foreach (MapMobSpawnData mob in map.Data.Mobs)
                    {
                        map.Mobs.Add(new Mob(mob));
                    }

                    foreach (MapNpcSpawnData npc in map.Data.Npcs)
                    {
                        map.Npcs.Add(new Npc(npc));
                    }

                    foreach (MapPortalData portal in map.Data.Portals)
                    {
                        map.Portals.Add(new Portal(portal));
                    }

                    base.Add(mapleID, map);
                }

                return base[mapleID];
            }
        }

        public ChannelMaps(byte world, byte channel)
            : base()
        {
            this.World = world;
            this.Channel = channel;
        }
    }
    
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public ChannelMaps Maps { get; private set; }

        public ChannelServer(byte id, byte worldID, string label, short port)
            : base(label, port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Maps = new ChannelMaps(this.WorldID, this.ID);
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
        }

        protected override void SetClientAttributes(MapleClient client)
        {
            client.World = this.WorldID;
            client.Channel = this.ID;
        }

        public void Notify(string message, NoticeType type)
        {
            //foreach (Character character in this.Characters)
            //{
            //    character.Notify(message, type);
            //}
        }
    }
}
