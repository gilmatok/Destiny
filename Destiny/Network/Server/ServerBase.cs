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
        private string mLabel;
        private Acceptor mAcceptor;
        private PacketProcessor mProcessor;
        private List<MapleClient> mClients;

        public MigrationRegistery Migrations { get; private set; }

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
            mAcceptor = new Acceptor(port, this.OnClientAccepted);
            mProcessor = new PacketProcessor(mLabel);
            mClients = new List<MapleClient>();

            this.Migrations = new MigrationRegistery();

            this.SpawnHandlers();
        }

        public virtual void Start()
        {
            mAcceptor.Start();

            Logger.Write(LogLevel.Info, "{0} started on port {1}.", mLabel, this.Port);
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

            Logger.Write(LogLevel.Info, "{0} stopped.", mLabel);
        }

        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove);

            this.SetClientAttributes(client);

            mClients.Add(client);

            client.Handshake();

            Logger.Write(LogLevel.Info, "[{0}] Accepted client from {1}.", mLabel, client.Host);
        }

        protected void AddHandler(RecvOps operationCode, PacketHandler handler)
        {
            mProcessor.Add(operationCode, handler);
        }

        protected virtual void SetClientAttributes(MapleClient client) { }

        protected abstract void SpawnHandlers();
    }
}
