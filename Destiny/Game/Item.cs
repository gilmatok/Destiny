using Destiny.Core.IO;
using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public class Item
    {
        public static InventoryType GetInventory(int mapleID)
        {
            return (InventoryType)(mapleID / 1000000);
        }

        public int MapleID { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public Item(int mapleID, short quantity = 1)
        {
            this.MapleID = mapleID;
            this.Quantity = quantity;
        }

        public Item(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("maple_id");
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
                .WriteMapleString(string.Empty) // NOTE: Creator.
                .WriteShort(); // NOTE: Flags.
        }
    }
}
