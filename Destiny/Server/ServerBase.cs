using Destiny.Core.Network;
using Destiny.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

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
        private string m_label;
        private Acceptor m_acceptor;
        private PacketProcessor m_processor;
        private List<MapleClient> m_clients;

        public MigrationRegistery Migrations { get; private set; }

        public short Port
        {
            get
            {
                return m_acceptor.Port;
            }
        }

        protected ServerBase(string label, short port)
        {
            m_label = label;
            m_acceptor = new Acceptor(port, this.OnClientAccepted);
            m_processor = new PacketProcessor(m_label);
            m_clients = new List<MapleClient>();

            this.Migrations = new MigrationRegistery();

            this.SpawnHandlers();
        }

        public virtual void Start()
        {
            m_acceptor.Start();
        }

        public virtual void Stop()
        {
            m_acceptor.Stop();

            foreach (MapleClient client in m_clients)
            {
                client.Close();
            }
        }

        private void OnClientAccepted(Socket socket)
        {
            try
            {
                MapleClient client = new MapleClient(socket, m_processor, m_clients.Remove);

                this.SetClientAttributes(client);

                m_clients.Add(client);

                client.Handshake();

                Logger.Write(LogLevel.Info, "[{0}] Accepted client from {1}.", m_label, client.Host);
            }
            catch 
            {
                //?
            }
        }

        protected void AddHandler(RecvOps operationCode, PacketHandler handler)
        {
            m_processor.Add(operationCode, handler);
        }

        protected virtual void SetClientAttributes(MapleClient client) { }

        protected abstract void SpawnHandlers();
    }
}
