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
}
