using Destiny.Core.Network;
using System;
using System.Net.Sockets;
using Destiny.Maple;
using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Network;

namespace Destiny
{
    public sealed class MapleClient : Session
    {
        private PacketProcessor mProcessor;
        private Func<MapleClient, bool> mDeathAction;

        public Account Account { get; set; }
        public Character Character { get; set; }

        public byte Channel { get; set; }

        public MapleClient(Socket socket, PacketProcessor processor, Func<MapleClient, bool> deathAction)
            : base(socket)
        {
            mProcessor = processor;
            mDeathAction = deathAction;
        }

        protected override void Terminate()
        {
            if (this.Character != null)
            {
                this.Character.Save();

                this.Character.Map.Characters.Remove(this.Character);
            }

            mDeathAction(this);
        }

        protected override void Dispatch(byte[] buffer)
        {
            using (InPacket iPacket = new InPacket(buffer))
            {
                PacketHandler handler = mProcessor[iPacket.OperationCode];

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
                    Log.Hex("Unhandled {0} from {1} on {2}: \n", iPacket.ToArray(), iPacket.OperationCode.ToString(), this.Host, mProcessor.Label);
                }
            }
        }
    }
}
