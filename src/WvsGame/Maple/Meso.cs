using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;

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

        public override Packet GetShowGainPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.Message);

            oPacket
                .WriteByte((byte)MessageType.DropPickup)
                .WriteBool(true)
                .WriteByte() // NOTE: Unknown.
                .WriteInt(this.Amount)
                .WriteShort();

            return oPacket;
        }

        /*public static Packet GetShowMesoGainPacket(bool white, int ammount, bool inChat)
        {
            return Character.GetShowSidebarInfoPacket(MessageType.DropPickup, white, 0, ammount, inChat, 0, 0);
        }*/

    }
}