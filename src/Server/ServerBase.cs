using Destiny.Core.Network;
using Destiny.Network;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public abstract class ServerBase
    {
        protected Acceptor mAcceptor;
        protected List<MapleClient> mClients;
        protected PacketProcessor mProcessor;

        public string Label
        {
            get
            {
                return mProcessor.Label;
            }
        }

        public short Port
        {
            get
            {
                return mAcceptor.Port;
            }
        }

        public int Load
        {
            get
            {
                lock (mClients)
                {
                    return mClients.Count;
                }
            }
        }

        protected ServerBase(string label, short port)
        {
            mAcceptor = new Acceptor(port, this.OnClientAccepted);
            mClients = new List<MapleClient>();
            mProcessor = new PacketProcessor(label);

            this.SpawnHandlers();
        }

        public virtual void Start()
        {
            mAcceptor.Start();

            Log.Inform("{0} server started on port {1}.", this.Label, this.Port);
        }

        public virtual void Stop()
        {
            if (mAcceptor.Active)
            {
                mAcceptor.Stop();
            }

            MapleClient[] remainingClients = mClients.ToArray();

            foreach (MapleClient client in remainingClients)
            {
                client.Close();
            }

            Log.Warn("{0} server stopped.", this.Label);
        }

        protected abstract void SpawnHandlers();
        protected abstract void OnClientAccepted(Socket socket);

        public void AddClient(MapleClient client)
        {
            lock (mClients)
            {
                mClients.Add(client);
            }
        }

        public void RemoveClient(MapleClient client)
        {
            lock (mClients)
            {
                mClients.Remove(client);
            }
        }
    }
}
