using Destiny.Handler;
using Destiny.Network;

namespace Destiny.Server
{
    public sealed class LoginServer : ServerBase
    {
        public LoginServer(short port) : base("Login", port) { }

        protected override void RegisterHandlers()
        {
            this.RegisterHandler(RecvOpcode.LoginPassword, LoginHandler.HandleLoginPassword);
        }
    }
}
