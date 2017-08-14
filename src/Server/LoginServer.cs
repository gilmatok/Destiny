using Destiny.IO;

namespace Destiny.Server
{
    public sealed class LoginServer : ServerBase
    {
        public bool AutoRegister { get; private set; }
        public bool RequestPin { get; private set; }
        public bool RequestPic { get; private set; }
        public int MaxCharacters { get; private set; }
        public bool RequireStaffIP { get; private set; }

        public LoginServer(short port)
            : base("Login", port)
        {
            this.AutoRegister = Settings.GetBool("Server/AutoRegister");
            this.RequestPin = Settings.GetBool("Server/RequestPin");
            this.RequestPic = Settings.GetBool("Server/RequestPic");
            this.MaxCharacters = Settings.GetInt("Server/MaxCharacters");
            this.RequireStaffIP = Settings.GetBool("Server/RequireStaffIP");
        }
    }
}
