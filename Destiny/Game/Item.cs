using Destiny.Core.IO;
using Destiny.Server;
using Destiny.Utility;
using System;
using System.IO;

namespace Destiny.Game
{
    public class Item
    {
        public int MapleID { get; private set; }
        public short MaxSlotQuantity { get; private set; }
        public int SalePrice { get; private set; }

        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public virtual Item CachedReference
        {
            get
            {
                return MasterServer.Instance.Data.Items[this.MapleID];
            }
        }

        public Item(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.MaxSlotQuantity = reader.ReadInt16();
            this.SalePrice = reader.ReadInt32();
        }

        public Item(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("item_identifier");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }

        public Item(int mapleID)
        {
            this.MapleID = mapleID;
            this.MaxSlotQuantity = this.CachedReference.MaxSlotQuantity;
            this.SalePrice = this.CachedReference.SalePrice;
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
