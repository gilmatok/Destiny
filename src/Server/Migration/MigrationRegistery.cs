using System;
using System.Collections.Generic;

namespace Destiny.Server.Migration
{
    public sealed class MigrationRegistery : List<MigrationData>
    {
        public MigrationRegistery() : base() { }

        public int Validate(string host, int characterID)
        {
            lock (this)
            {
                for (int i = this.Count; i-- > 0;)
                {
                    MigrationData request = this[i];

                    if ((DateTime.Now - request.Expiry).Seconds > 30)
                    {
                        this.Remove(request);

                        continue;
                    }

                    if (request.Host == host && request.CharacterID == characterID)
                    {
                        this.Remove(request);

                        return request.AccountID;
                    }
                }
            }

            return -1;
        }
    }
}
