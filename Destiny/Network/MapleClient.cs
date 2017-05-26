using Destiny.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Destiny.Game;
using Destiny.Core.IO;

namespace Destiny.Network
{
    public sealed class MapleClient : Session
    {
        private PacketProcessor mProcessor;
        private Func<MapleClient, bool> mDeathAction;

        public Account Account { get; set; }
        public Character Character { get; set; }

        public byte World { get; set; }
        public byte Channel { get; set; }

        public MapleClient(Socket socket, PacketProcessor processor, Func<MapleClient, bool> deathAction)
            : base(socket)
        {
            mProcessor = processor;
            mDeathAction = deathAction;
        }

        protected override void Terminate()
        {
            if (this.Account != null)
            {
                this.Account.Save();
            }

            if (this.Character != null)
            {
                this.Character.Save();
            }

            mDeathAction(this);
        }

        protected override void Dispatch(InPacket iPacket)
        {
            PacketHandler handler = mProcessor[iPacket.OperationCode];

            if (handler != null)
            {
                handler(this, iPacket);
            }
            else
            {
                Logger.Write(LogLevel.Warning, "[{0}] Unhandled packet from {1}: {2}", mProcessor.Label, this.Host, iPacket.ToString());
            }
        }
    }
}
