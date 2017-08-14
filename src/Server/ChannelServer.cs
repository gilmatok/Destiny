using Destiny.Maple.Maps;
using Destiny.Network;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }
        public PlayerStorage Players { get; private set; }

        public ChannelServer(byte id, byte worldID, short port)
            : base(string.Format("{0}-{1}", "Scania", id), port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory();
            this.Players = new PlayerStorage();
        }

        protected override void OnClientAccepted(MapleClient client)
        {
            client.World = this.WorldID;
            client.Channel = this.ID;

            base.OnClientAccepted(client);
        }
    }
}
