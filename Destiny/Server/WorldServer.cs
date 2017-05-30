using Destiny.Utility;
using System;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class WorldServer
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public ChannelServer[] Channels { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }

        private List<MigrationData> mMigrationRequests;

        public WorldStatus Status
        {
            get
            {
                return WorldStatus.Normal; // NOTE: Unless someone wants to impose a maximum registered users, this is useless.
            }
        }

        public WorldServer(CWorld config)
        {
            this.ID = config.ID;
            this.Name = config.Name;
            this.Channels = new ChannelServer[config.Channels];

            short port = config.Port;

            for (byte id = 0; id < config.Channels; id++)
            {
                this.Channels[id] = new ChannelServer(id, this.ID, port);

                port++;
            }

            this.Flag = config.Flag;
            this.EventMessage = config.EventMessage;
            this.TickerMessage = config.TickerMessage;

            mMigrationRequests = new List<MigrationData>();
        }

        public void Start()
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Start();
            }

            Logger.Write(LogLevel.Info, "WorldServer {0} started with {1} channels.", this.Name, this.Channels.Length);
        }

        public void Stop()
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Stop();
            }

            Logger.Write(LogLevel.Info, "WorldServer {0} stopped.", this.Name);
        }

        public void AddMigrationRequest(string host, int accountID, int characterID)
        {
            lock (mMigrationRequests)
            {
                mMigrationRequests.Add(new MigrationData(host, accountID, characterID));
            }
        }

        public int EligableMigration(string host, int characterID)
        {
            lock (mMigrationRequests)
            {
                for (int i = mMigrationRequests.Count; i-- > 0;)
                {
                    MigrationData itr = mMigrationRequests[i];

                    if ((DateTime.Now - itr.Expiry).Seconds > 30)
                    {
                        mMigrationRequests.Remove(itr);

                        continue;
                    }

                    if (itr.Host == host && itr.CharacterID == characterID)
                    {
                        mMigrationRequests.Remove(itr);

                        return itr.AccountID;
                    }
                }
            }

            return 0;
        }

        public void Notify(string message, NoticeType type)
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Notify(message, type);
            }
        }
    }
}
