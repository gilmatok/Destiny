using System;

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
}
