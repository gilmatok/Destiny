using Destiny.Core.Network;
using System;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class MigrationData
    {
        public string Host { get; private set; }
        public int AccountID { get; private set; }
        public int CharacterID { get; private set; }
        public DateTime Expiry { get; private set; }

        public MigrationData(string host, int accountID, int characterID)
        {
            this.Host = host;
            this.AccountID = accountID;
            this.CharacterID = characterID;
            this.Expiry = DateTime.Now;
        }
    }

    public sealed class MigrationRegistery
    {
        private List<MigrationData> mRequests;

        public MigrationRegistery()
        {
            mRequests = new List<MigrationData>();
        }

        public void Add(string host, int accountID, int characterID)
        {
            lock (mRequests)
            {
                mRequests.Add(new MigrationData(host, accountID, characterID));
            }
        }

        public int Validate(string host, int characterID)
        {
            lock (mRequests)
            {
                for (int i = mRequests.Count; i-- > 0;)
                {
                    MigrationData request = mRequests[i];

                    if ((DateTime.Now - request.Expiry).Seconds > 30)
                    {
                        mRequests.Remove(request);

                        continue;
                    }

                    if (request.Host == host && request.CharacterID == characterID)
                    {
                        mRequests.Remove(request);

                        return request.AccountID;
                    }
                }
            }

            return -1;
        }
    }

    public abstract class ServerBase
    {
        private Acceptor mAcceptor;

        public string Label { get; private set; }
        public List<MapleClient> Clients { get; private set; }
        public MigrationRegistery Migrations { get; private set; }

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
            this.Migrations = new MigrationRegistery();
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
