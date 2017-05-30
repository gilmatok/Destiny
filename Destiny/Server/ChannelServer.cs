using Destiny.Game.Characters;
using Destiny.Handler;
using Destiny.Network;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public MapFactory Maps { get; private set; }
        public ChannelCharacters Characters { get; private set; }

        public ChannelServer(byte id, byte worldID, short port)
            : base("Channel", port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Maps = new MapFactory(this.WorldID, this.ID);
            this.Characters = new ChannelCharacters(this.WorldID, this.ID);
        }

        protected override void ClientAdded(MapleClient client)
        {
            client.World = this.WorldID;
            client.Channel = this.ID;
        }

        protected override void RegisterHandlers()
        {
            this.RegisterHandler(RecvOpcode.MigrateIn, ServerHandler.OnMigrateIn);
            this.RegisterHandler(RecvOpcode.TransferFieldRequest, UserHandler.OnTransferFieldRequest);
            this.RegisterHandler(RecvOpcode.UserChat, UserHandler.OnChat);
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
