using Destiny.Core.Collections;
using Destiny.Game.Characters;
using Destiny.Game.Maps;
using Destiny.Network;
using Destiny.Network.Handler;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class ChannelCharacters : EnumerationHelper<int, Character>
    {
        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public ChannelCharacters(byte world, byte channel)
        {
            this.World = world;
            this.Channel = channel;
        }

        public override IEnumerator<Character> GetEnumerator()
        {
            foreach (Map map in MasterServer.Instance.Worlds[this.World].Channels[this.Channel].Maps.Values)
            {
                foreach (Character character in map.Characters)
                {
                    yield return character;
                }
            }
        }

        public override int GetKeyForObject(Character item)
        {
            return item.ID;
        }
    }

    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }
        public ChannelCharacters Characters { get; private set; }

        public ChannelServer(byte id, byte worldID, string worldName, short port)
            : base(string.Format("{0}-{1}", worldName, id), port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory(this.WorldID, this.ID);
            this.Characters = new ChannelCharacters(this.WorldID, this.ID);
        }

        protected override void SpawnHandlers()
        {
            this.AddHandler(RecvOps.MigrateIn, ServerHandler.HandleMigrateChannel);
            this.AddHandler(RecvOps.TransferFieldRequest, UserHandler.OnTransferFieldRequest);
            this.AddHandler(RecvOps.ChangeChannel, ServerHandler.HandleChangeChannel);
            this.AddHandler(RecvOps.EnterCashShop, ServerHandler.HandleCashShop);
            this.AddHandler(RecvOps.EnterMts, ServerHandler.HandleMTS);
            this.AddHandler(RecvOps.UserChat, UserHandler.OnChat);
            this.AddHandler(RecvOps.UserMove, UserHandler.OnMove);
            this.AddHandler(RecvOps.ItemMove, InventoryHandler.HandleItemMove);
            this.AddHandler(RecvOps.NpcMove, NpcHandler.HandleNpcMove);
            this.AddHandler(RecvOps.MobMove, MobHandler.OnMobMove);
        }

        protected override void SetClientAttributes(MapleClient client)
        {
            client.World = this.WorldID;
            client.Channel = this.ID;
        }

        public void Notify(string message, NoticeType type)
        {
            foreach (Character character in this.Characters)
            {
                character.Notify(message, type);
            }
        }
    }
}
