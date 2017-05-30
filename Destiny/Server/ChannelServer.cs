using Destiny.Handler;
using Destiny.Network;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public MapFactory Maps { get; private set; }

        public ChannelServer(byte id, byte worldID, short port)
            : base("Channel", port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Maps = new MapFactory(this.WorldID, this.ID);
        }

        protected override void ClientAdded(MapleClient client)
        {
            client.World = this.WorldID;
            client.Channel = this.ID;
        }

        protected override void RegisterHandlers()
        {
            this.RegisterHandler(RecvOpcode.MigrateIn, ServerHandler.HandleMigrateChannel);
            this.RegisterHandler(RecvOpcode.UserChat, UserHandler.HandleUserChat);
        }
    }
}
