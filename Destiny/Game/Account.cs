using Destiny.Utility;

namespace Destiny.Game
{
    public sealed class Account
    {
        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }
        public GmLevel GmLevel { get; private set; }

        public Account(DatabaseQuery query)
        {
            this.ID = query.GetInt("account_id");
            this.Username = query.GetString("username");
            this.Password = query.GetString("password");
            this.Salt = query.GetString("salt");
            this.GmLevel = (GmLevel)query.GetByte("gm_level");
        }

        public void Save()
        {

        }
    }
}