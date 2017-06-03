using Destiny.Network;
using Destiny.Network.Handler;
using Destiny.Utility;

namespace Destiny.Server
{
    public sealed class LoginServer : ServerBase
    {
        public bool AutoRegister { get; private set; }
        public bool RequestPin { get; private set; }
        public bool RequestPic { get; private set; }

        public LoginServer(CLogin config)
            : base("Login", config.Port)
        {
            this.AutoRegister = config.AutoRegister;
            this.RequestPin = config.RequestPin;
            this.RequestPic = config.RequestPic;
        }

        public override void Start()
        {
            base.Start();

            Logger.Write(LogLevel.Info, "LoginServer started on port {0}.", this.Port);
        }

        protected override void SpawnHandlers()
        {
            this.AddHandler(RecvOps.LoginPassword, LoginHandler.HandleLoginPassword);
            this.AddHandler(RecvOps.WorldList, LoginHandler.HandleWorldList);
            this.AddHandler(RecvOps.WorldRelist, LoginHandler.HandleWorldList);
            this.AddHandler(RecvOps.CheckUserLimit, LoginHandler.HandleCheckUserLimit);
            this.AddHandler(RecvOps.WorldSelect, LoginHandler.HandleWorldSelect);
            this.AddHandler(RecvOps.CharacterNameCheck, LoginHandler.HandleCharacterNameCheck);
            this.AddHandler(RecvOps.CharacterCreate, LoginHandler.HandleCharacterCreation);
            this.AddHandler(RecvOps.CharacterSelect, LoginHandler.HandleCharacterSelection);
        }
    }
}
