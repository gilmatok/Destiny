using Destiny.Data;
using Destiny.Network;
using System;
using System.Data;

namespace Destiny.Maple
{
    public sealed class Account
    {
        public GameClient Client { get; private set; }

        public int ID { get; private set; }
        public string Username { get; set; }
        public Gender Gender { get; set; }
        public bool IsMaster { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Creation { get; set; }

        private bool Assigned { get; set; }

        public Account(GameClient client)
        {
            this.Client = client;
        }

        public void Load(int accountID)
        {
            Datum datum = new Datum("accounts");

            try
            {
                datum.Populate("ID = {0}", accountID);
            }
            catch (RowNotInTableException)
            {
                throw new NoAccountException();
            }

            this.ID = (int)datum["ID"];
            this.Assigned = true;

            this.Username = (string)datum["Username"];
            this.Gender = (Gender)datum["Gender"];
            this.IsMaster = (bool)datum["IsMaster"];
            this.Birthday = (DateTime)datum["Birthday"];
            this.Creation = (DateTime)datum["Creation"];
        }
    }
}
