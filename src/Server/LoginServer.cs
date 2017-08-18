using System.Net.Sockets;
using Destiny.IO;
using Destiny.Packets;

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
            mProcessor.Add(ClientOperationCode.AccountLogin, PacketHandlers.HandleAccountLogin);
            mProcessor.Add(ClientOperationCode.WorldList, PacketHandlers.HandleWorldList);
            mProcessor.Add(ClientOperationCode.WorldRelist, PacketHandlers.HandleWorldList);
            mProcessor.Add(ClientOperationCode.WorldStatus, PacketHandlers.HandleWorldStatus);
            mProcessor.Add(ClientOperationCode.WorldSelect, PacketHandlers.HandleWorldSelect);
            mProcessor.Add(ClientOperationCode.CharacterNameCheck, PacketHandlers.HandleCharacterNameCheck);
            mProcessor.Add(ClientOperationCode.CharacterCreate, PacketHandlers.HandleCharacterCreation);
            mProcessor.Add(ClientOperationCode.CharacterDelete, PacketHandlers.HandleCharacterDeletion);
            mProcessor.Add(ClientOperationCode.CharacterSelect, PacketHandlers.HandleCharacterSelection); // TODO: Should we split these handlers?
            mProcessor.Add(ClientOperationCode.CharacterSelectRequestPic, PacketHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.CharacterSelectRegisterPic, PacketHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.SelectCharacterByVAC, PacketHandlers.HandleCharacterSelection);
            mProcessor.Add(ClientOperationCode.RegisterPicFromVAC, PacketHandlers.HandleCharacterSelection);
        }

        protected override void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, this, mProcessor);

            client.Handshake();
        }
    }
}
