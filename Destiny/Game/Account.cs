using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public sealed class Account
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public Account(DatabaseQuery query)
        {
            this.ID = query.GetInt("account_id");
            this.Username = query.GetString("username");
            this.Password = query.GetString("password");
            this.Salt = query.GetString("salt");
        }

        public void Save()
        {

        }
    }
}
