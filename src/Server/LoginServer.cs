using System.Net.Sockets;
using Destiny.IO;
using Destiny.Core.Network;
using Destiny.Handlers;

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

        protected override void SpawnHandlers()
        {
            mProcessor.Add(ClientOperationCode.AccountLogin, LoginHandlers.HandleAccountLogin);
            mProcessor.Add(ClientOperationCode.WorldList, LoginHandlers.HandleWorldList);
            mProcessor.Add(ClientOperationCode.WorldRelist, LoginHandlers.HandleWorldList);
            mProcessor.Add(ClientOperationCode.WorldStatus, LoginHandlers.HandleWorldStatus);
            mProcessor.Add(ClientOperationCode.WorldSelect, LoginHandlers.HandleWorldSelect);
            mProcessor.Add(ClientOperationCode.CharacterNameCheck, LoginHandlers.HandleCharacterNameCheck);
            mProcessor.Add(ClientOperationCode.CharacterCreate, LoginHandlers.HandleCharacterCreation);
            mProcessor.Add(ClientOperationCode.CharacterDelete, LoginHandlers.HandleCharacterDeletion);
            mProcessor.Add(ClientOperationCode.CharacterSelect, LoginHandlers.HandleCharacterSelection); // TODO: Should we split these handlers?
            mProcessor.Add(ClientOperationCode.CharacterSelectRequestPic, LoginHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.CharacterSelectRegisterPic, LoginHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.SelectCharacterByVAC, LoginHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.RegisterPicFromVAC, LoginHandlers.HandleCharacterSelection);
        }

        protected override void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, this, mProcessor);

            client.Handshake();
        }
    }
}
