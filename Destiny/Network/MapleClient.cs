using Destiny.Core.Network;
using System;
using System.Net.Sockets;
using Destiny.Game;
using Destiny.Core.IO;
using Destiny.Game.Characters;

namespace Destiny.Network
{
    public sealed class MapleClient : Session
    {
        public Account Account { get; set; }
        public Character Character { get; set; }

        public string LastUsername { get; set; }
        public string LastPassword { get; set; }

        public byte World { get; set; }
        public byte Channel { get; set; }

        private PacketProcessor mProcessor;
        private Func<MapleClient, bool> mDeathAction;

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
                        Logger.Exception(ex);
                    }
                }
                else
                {
                    Logger.Write(LogLevel.Warning, "[{0}] Unhandled packet from {1}: {2}", mProcessor.Label, this.Host, iPacket.ToString());
                }
            }
        }

        public void Migrate(bool valid, short port)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.MigrateCommand))
            {
                oPacket
                    .WriteBool(valid)
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(port);

                this.Send(oPacket);
            }
        }
    }
}
