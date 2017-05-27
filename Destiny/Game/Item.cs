using Destiny.Core.IO;
using Destiny.Utility;
using System;
using System.IO;

namespace Destiny.Game
{
    public class Item
    {
        public int Identifier { get; private set; }
        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public ItemType Type
        {
            get
            {
                return (ItemType)(this.Identifier / 1000000);
            }
        }

        public Item(DatabaseQuery query)
        {
            this.Identifier = query.GetInt("item_identifier");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }

        public virtual void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(2)
                .WriteInt(this.Identifier)
                .WriteBool(false)
                .WriteLong() // TODO: Expiration.
                .WriteShort(this.Quantity)
                .WriteString(string.Empty) // NOTE: Creator.
                .WriteShort(); // NOTE: Flags.
        }
    }
}
