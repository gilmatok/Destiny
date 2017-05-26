using Destiny.Core.Network;
using Destiny.Network;
using Destiny.Packet;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public abstract class ServerBase
    {
        private string mLabel;
        private Acceptor mAcceptor;
        private List<MapleClient> mClients;
        private PacketProcessor mProcessor;

        public short Port
        {
            get
            {
                return mAcceptor.Port;
            }
        }

        protected ServerBase(string label, short port)
        {
            mLabel = label;
            mAcceptor = new Acceptor(port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;

            mClients = new List<MapleClient>();

            mProcessor = new PacketProcessor(mLabel);

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

            Logger.Write(LogLevel.Connection, "[{0}] Accepted client {1}.", mLabel, client.Host);
        }

        public virtual void Start()
        {
            mAcceptor.Start();
        }

        public virtual void Stop()
        {
            mAcceptor.Stop();
            mClients.ForEach(c => c.Close());
        }
    }
}
