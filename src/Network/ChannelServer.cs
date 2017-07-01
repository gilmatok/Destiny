using Destiny.Maple.Maps;

namespace Destiny.Network
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }

        public ChannelServer(byte id, short port)
            : base(string.Format("{0}-{1}", MasterServer.World.Name, id), port)
        {
            this.ID = id;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory(this.ID);
        }

        protected override void OnClientAccepted(MapleClient client)
        {
            client.Channel = this.ID;

            base.OnClientAccepted(client);
        }
    }
}
