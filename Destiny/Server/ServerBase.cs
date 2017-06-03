using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Network;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public abstract class ServerBase
    {
        private string mLabel;
        private Acceptor mAcceptor;
        private PacketProcessor mProcessor;
        private List<MapleClient> mClients;

        public short Port
        {
            get
            {
                return mAcceptor.Port;
            }
        }

        protected ServerBase(string label, short port)
        {
            mLabel = label;
            mAcceptor = new Acceptor(port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;
            mProcessor = new PacketProcessor(mLabel);
            mClients = new List<MapleClient>();

            this.SpawnHandlers();
        }

        public virtual void Start()
        {
            mAcceptor.Start();
        }

        public virtual void Stop()
        {
            mAcceptor.Stop();

            foreach (MapleClient client in mClients)
            {
                client.Close();
            }
        }

        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove);

            this.SetClientAttributes(client);

            mClients.Add(client);

            using (OutPacket oPacket = new OutPacket(14, 16))
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteMapleString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                client.SendRaw(oPacket.ToArray());
            }

            Logger.Write(LogLevel.Info, "[{0}] Accepted client from {1}.", mLabel, client.Host);
        }

        protected void AddHandler(RecvOps operationCode, PacketHandler handler)
        {
            mProcessor.Add(operationCode, handler);
        }

        protected virtual void SetClientAttributes(MapleClient client) { }

        protected abstract void SpawnHandlers();
    }
}
