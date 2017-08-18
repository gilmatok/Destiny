using Destiny.Maple.Maps;
using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Packets;

namespace Destiny.Maple
{
    public sealed class Meso : Drop
    {
        public int Amount { get; private set; }

        public Meso(int amount)
             : base()
        {
            this.Amount = amount;
        }

        public override OutPacket GetShowGainPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.Message);

            oPacket
                .WriteByte()
                .WriteShort(1)
                .WriteInt(this.Amount)
                .WriteShort();

            return oPacket;
        }
    }
}
