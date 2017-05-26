using Destiny.Core.Network;
using Destiny.Network;
using Destiny.Packet;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public abstract class ServerBase
    {
        private Acceptor mAcceptor;
        private List<MapleClient> mClients;
        private PacketProcessor mProcessor;

        protected ServerBase(string label, short port)
        {
            mAcceptor = new Acceptor(port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;

            mClients = new List<MapleClient>();

            mProcessor = new PacketProcessor(label);

            this.RegisterHandlers();
        }

        protected abstract void RegisterHandlers();
        
        protected void RegisterHandler(RecvOpcode operationCode, PacketHandler handler)
        {
            mProcessor.Add((short)operationCode, handler);
        }

        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove);

            mClients.Add(client);

            client.SendRaw(LoginPacket.Handshake());

            Logger.Write(LogLevel.Connection, "Accepted client {0}.", client.Host);
        }

        public virtual void Run()
        {
            mAcceptor.Start();
        }

        public void Shutdown()
        {
            mAcceptor.Stop();
            mClients.ForEach(m => m.Close());
        }
    }
}
