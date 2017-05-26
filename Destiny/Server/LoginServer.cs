using Destiny.Handler;
using Destiny.Network;

namespace Destiny.Server
{
    public sealed class LoginServer : ServerBase
    {
        public LoginServer(short port) : base("Login", port) { }

        public override void Start()
        {
            base.Start();

            Logger.Write(LogLevel.Info, "LoginServer started on port {0}.", this.Port);
        }

        protected override void RegisterHandlers()
        {
            this.RegisterHandler(RecvOpcode.LoginPassword, LoginHandler.HandleLoginPassword);
            this.RegisterHandler(RecvOpcode.WorldList, LoginHandler.HandleWorldList);
        }
    }
}
