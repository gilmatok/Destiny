using Destiny.Core.Network;
using System.Collections.Generic;

namespace Destiny.Network
{
    public abstract class ServerBase
    {
        private Acceptor mAcceptor;

        public string Label { get; private set; }
        public List<MapleClient> Clients { get; private set; }

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
                lock (this.Clients)
                {
                    return this.Clients.Count;
                }
            }
        }

        protected ServerBase(string label, short port)
        {
            mAcceptor = new Acceptor(port, this.OnClientAccepted);

            this.Label = label;
            this.Clients = new List<MapleClient>();
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

            MapleClient[] remainingClients = this.Clients.ToArray();

            foreach (MapleClient client in remainingClients)
            {
                client.Close();
            }

            Log.Warn("{0} server stopped.", this.Label);
        }

        protected virtual void OnClientAccepted(MapleClient client)
        {
            this.Clients.Add(client);

            client.Handshake();

            Log.Inform("Accepted client from {0} on {1} server.", client.Host, this.Label);
        }
    }
}
