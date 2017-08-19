using System.Net.Sockets;
using Destiny.Maple.Maps;
using Destiny.Server.Migration;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public WorldServer World { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }

        public ChannelServer(byte id, WorldServer world, short port)
            : base(string.Format("{0}-{1}", world.Name, id), port)
        {
            this.ID = id;
            this.World = world;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory();
        }

        protected override void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, this)
            {
                WorldID = this.World.ID,
                ChannelID = this.ID
            };

            client.Handshake();
        }
    }
}
