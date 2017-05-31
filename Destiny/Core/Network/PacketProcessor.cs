using Destiny.Core.IO;

namespace Destiny.Network
{
    public delegate void PacketHandler(MapleClient client, InPacket iPacket);

    public sealed class PacketProcessor
    {
        public string Label { get; private set; }

        private PacketHandler[] mHandlers;
        private int mCount;

        public int Count
        {
            get
            {
                return mCount;
            }
        }

        public PacketHandler this[short operationCode]
        {
            get
            {
                return mHandlers[operationCode];
            }
        }

        public PacketProcessor(string label)
        {
            this.Label = label;

            mHandlers = new PacketHandler[0xFFFF + 1];
        }

        public void Add(RecvOps operationCode, PacketHandler handler)
        {
            mHandlers[(short)operationCode] = handler;
            mCount++;
        }
    }
}
