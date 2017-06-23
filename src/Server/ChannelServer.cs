using Destiny.Maple.Maps;

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

        protected override void OnClientAccepted(MapleClient client)
        {
            client.Channel = this.ID;

            base.OnClientAccepted(client);
        }
    }
}
