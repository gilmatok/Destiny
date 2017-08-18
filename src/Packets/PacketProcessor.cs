using Destiny.Core.IO;
using System.Collections.Generic;

namespace Destiny.Packets
{
    public delegate void PacketHandler(MapleClient client, InPacket iPacket);

    public sealed class PacketProcessor : Dictionary<ClientOperationCode, PacketHandler>
    {
        public string Label { get; private set; }

        public PacketProcessor(string label)
             : base()
        {
            this.Label = label;
        }
    }
}