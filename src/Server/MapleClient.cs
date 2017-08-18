using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Packets;
using Destiny.Server;
using System;
using System.Net.Sockets;

namespace Destiny
{
    public sealed class MapleClient : Session
    {
        private ServerBase mParentServer;
        private PacketProcessor mProcessor;

        public Account Account { get; set; }
        public Character Character { get; set; }

        public string LastUsername { get; set; }
        public string LastPassword { get; set; }

        public byte World { get; set; }
        public byte Channel { get; set; }
        public bool VAC { get; set; }

        public MapleClient(Socket socket, ServerBase parentServer, PacketProcessor processor)
            : base(socket)
        {
            mParentServer = parentServer;
            mProcessor = processor;

            mParentServer.AddClient(this);
        }

        protected override void Terminate()
        {
            if (this.Character != null)
            {
                this.Character.Save();
                this.Character.LastNpc = null;
                this.Character.Map.Characters.Remove(this.Character);
            }

            mParentServer.RemoveClient(this);
        }

        protected override void Dispatch(InPacket iPacket)
        {
            PacketHandler handler;

            mProcessor.TryGetValue(iPacket.OperationCode, out handler);

            if (handler != null)
            {
                try
                {
                    handler(this, iPacket);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            else
            {
                Log.Warn("Missing handler for {0}.", iPacket.OperationCode.ToString());
            }
        }
    }
}