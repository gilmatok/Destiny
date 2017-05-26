using System;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class WorldServer
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public ChannelServer[] Channels { get; private set; }

        private List<MigrationData> mMigrationRequests;

        public WorldServer(byte id, short port, int channels)
        {
            this.ID = id;
            this.Name = Constants.WorldNames[id];
            this.Channels = new ChannelServer[channels];

            for (int i = 0; i < channels; i++)
            {
                this.Channels[i] = new ChannelServer((byte)i, id, port);

                port++;
            }

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
    }
}
