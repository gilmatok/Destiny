using Destiny.Core.Network;
using Destiny.Handler;

namespace Destiny.Server
{
    public sealed class LoginServer : ServerBase
    {
        public LoginServer(short port) : base("Login", port) { }
        
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
