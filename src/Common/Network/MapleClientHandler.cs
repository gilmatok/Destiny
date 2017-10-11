using Destiny.IO;
using Destiny.Security;
using System.Net.Sockets;

namespace Destiny.Network
{
    public abstract class MapleClientHandler : ClientHandler<ClientOperationCode, ServerOperationCode, MapleCryptograph>
    {
        public MapleClientHandler(Socket socket) : base(socket, "Client") { }

        protected override void Initialize()
        {
            byte[] initialization = this.Cryptograph.Initialize();

            this.Socket.Send(initialization);

            switch (Packet.LogLevel)
            {
                case LogLevel.Name:
                    Log.Inform("Sent Initialization packet (unencrypted).");
                    break;

                case LogLevel.Full:
                    Log.Hex("Sent Initialization packet (unencrypted): ", initialization);
                    break;
            }
        }
    }
}
