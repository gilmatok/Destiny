using Destiny.Handler;
using Destiny.Network;
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

        protected override void RegisterHandlers()
        {
            this.RegisterHandler(RecvOpcode.LoginPassword, LoginHandler.HandleLoginPassword);
            this.RegisterHandler(RecvOpcode.WorldList, LoginHandler.HandleWorldList);
            this.RegisterHandler(RecvOpcode.WorldRelist, LoginHandler.HandleWorldList);
            this.RegisterHandler(RecvOpcode.CheckUserLimit, LoginHandler.HandleCheckUserLimit);
            this.RegisterHandler(RecvOpcode.WorldSelect, LoginHandler.HandleWorldSELECT);
            this.RegisterHandler(RecvOpcode.CharacterNameCheck, LoginHandler.HandleCharacterNameCheck);
            this.RegisterHandler(RecvOpcode.CharacterCreate, LoginHandler.HandleCharacterCreation);
            this.RegisterHandler(RecvOpcode.CharacterSelect, LoginHandler.HandleCharacterSelection);
        }
    }
}
