using Destiny.Core.IO;
using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public class Item
    {
        public int MapleID { get; private set; }
        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public ItemType Type
        {
            get
            {
                return (ItemType)(this.MapleID / 1000000);
            }
        }

        public Item(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("maple_id");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }

        public virtual void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(2)
                .WriteInt(this.MapleID)
                .WriteBool(false)
                .WriteLong() // TODO: Expiration.
                .WriteShort(this.Quantity)
                .WriteString(string.Empty) // NOTE: Creator.
                .WriteShort(); // NOTE: Flags.
        }
    }
}
