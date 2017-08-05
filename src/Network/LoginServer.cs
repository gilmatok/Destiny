using Destiny.IO;

namespace Destiny.Network
{
    public sealed class LoginServer : ServerBase
    {
        public bool AutoRegister { get; private set; }
        public int MaxCharacters { get; private set; }

        public LoginServer(short port)
            : base("Login", port)
        {
            this.AutoRegister = Settings.GetBool("Server/AutoRegister");
            this.MaxCharacters = Settings.GetInt("Server/MaxCharacters");
        }
    }
}
