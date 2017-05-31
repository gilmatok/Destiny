using Destiny.Core.Network;
using Destiny.Network;
using Destiny.Network.Handler;
using Destiny.Network.Packet;
using Destiny.Utility;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public sealed class ShopServer
    {
        public short Port { get; private set; }
        public MigrationRegistery Migrations { get; private set; }

        private Acceptor mAcceptor;
        private List<MapleClient> mClients;
        private PacketProcessor mProcessor;

        public ShopServer(CShop config)
        {
            this.Port = config.Port;
            this.Migrations = new MigrationRegistery();

            mAcceptor = new Acceptor(this.Port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;

            mClients = new List<MapleClient>();

            this.SpawnHandlers();
        }

        public void Start()
        {
            mAcceptor.Start();

            Logger.Write(LogLevel.Info, "ShopServer started on port {0}.", mAcceptor.Port);
        }

        public void Stop()
        {
            mAcceptor.Stop();

            foreach (MapleClient client in mClients)
            {
                client.Close();
            }
        }

        private void SpawnHandlers()
        {
            mProcessor = new PacketProcessor("Shop");

            mProcessor.Add(RecvOps.MigrateIn, ServerHandler.HandleMigrateCashShop);
            mProcessor.Add(RecvOps.TransferFieldRequest, ShopHandler.OnTransferFieldRequest);
        }

        // TODO: We need to get the last world/channel the client was in before the migration
        // to figure out where to send it back after it leaves the Cash Shop. For now, leave it
        // at world 0 channel 0.
        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove)
            {
                World = 0,
                Channel = 0
            };

            mClients.Add(client);

            client.SendRaw(CommonPacket.Handshake());

            Logger.Write(LogLevel.Connection, "[Shop] Accepted client {0}.", client.Host);
        }
    }
}
