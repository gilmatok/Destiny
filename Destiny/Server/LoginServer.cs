using Destiny.Core.Network;
using Destiny.Network;
using Destiny.Network.Handler;
using Destiny.Network.Packet;
using Destiny.Utility;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public sealed class LoginServer
    {
        public bool AutoRegister { get; private set; }
        public bool RequestPin { get; private set; }
        public bool RequestPic { get; private set; }

        private Acceptor mAcceptor;
        private List<MapleClient> mClients;
        private PacketProcessor mProcessor;

        public LoginServer(CLogin config)
        {
            this.AutoRegister = config.AutoRegister;
            this.RequestPin = config.RequestPin;
            this.RequestPic = config.RequestPic;

            mAcceptor = new Acceptor(config.Port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;

            mClients = new List<MapleClient>();

            this.SpawnHandlers();
        }

        public void Start()
        {
            mAcceptor.Start();

            Logger.Write(LogLevel.Info, "LoginServer started on port {0}.", mAcceptor.Port);
        }

        public void Stop()
        {
            mAcceptor.Stop();

            foreach (MapleClient client in mClients)
            {
                client.Close();
            }
        }

        private void SpawnHandlers()
        {
            mProcessor = new PacketProcessor("Login");

            mProcessor.Add(RecvOps.LoginPassword, LoginHandler.HandleLoginPassword);
            mProcessor.Add(RecvOps.WorldList, LoginHandler.HandleWorldList);
            mProcessor.Add(RecvOps.WorldRelist, LoginHandler.HandleWorldList);
            mProcessor.Add(RecvOps.CheckUserLimit, LoginHandler.HandleCheckUserLimit);
            mProcessor.Add(RecvOps.WorldSelect, LoginHandler.HandleWorldSELECT);
            mProcessor.Add(RecvOps.CharacterNameCheck, LoginHandler.HandleCharacterNameCheck);
            mProcessor.Add(RecvOps.CharacterCreate, LoginHandler.HandleCharacterCreation);
            mProcessor.Add(RecvOps.CharacterSelect, LoginHandler.HandleCharacterSelection);
        }

        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove);

            mClients.Add(client);

            client.SendRaw(CommonPacket.Handshake());

            Logger.Write(LogLevel.Connection, "[Login] Accepted client {0}.", client.Host);
        }
    }
}
