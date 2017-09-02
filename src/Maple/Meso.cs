using Destiny.Maple.Maps;
using Destiny.Core.Network;

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
                .WriteByte((byte)MessageType.DropPickup)
                .WriteBool(true)
                .WriteByte() // NOTE: Unknown.
                .WriteInt(this.Amount)
                .WriteShort();

            return oPacket;
        }
    }
}
