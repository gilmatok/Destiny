using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public sealed class Account
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public int PinCode { get; set; }
        public Gender Gender { get; set; }
        public byte GmLevel { get; set; }
        public bool IsBanned { get; set; }
        public DateTime BanExpiration { get; set; }
        public byte BanReason { get; set; }
        public string BanReasonMessage { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime QuietBanExpiration { get; set; }
        public byte QuietBanReason { get; set; }
        public DateTime CreationDate { get; set; }

        public Account(DatabaseQuery query)
        {
            this.ID = query.GetInt("account_id");
            this.Username = query.GetString("username");
            this.Password = query.GetString("password");
            this.PasswordSalt = query.GetString("password_salt");
            this.PinCode = query.GetInt("pin_code");
            this.Gender = (Gender)query.GetByte("gender");
            this.GmLevel = query.GetByte("gm_level");
            this.IsBanned = query.GetBool("banned");
            this.BanExpiration = query.GetDateTime("ban_expire");
            this.BanReason = query.GetByte("ban_reason");
            this.BanReasonMessage = query.GetString("ban_reason_message");
            this.LastLogin = query.GetDateTime("last_login");
            this.QuietBanExpiration = query.GetDateTime("quiet_ban_expire");
            this.QuietBanReason = query.GetByte("quiet_ban_reason");
            this.CreationDate = query.GetDateTime("creation_date");
        }

        public void Save()
        {

        }
    }
}
