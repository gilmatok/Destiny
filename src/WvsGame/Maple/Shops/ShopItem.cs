using Destiny.Data;
using Destiny.Maple.Data;
using System;
using Destiny.IO;
using Destiny.Data;

namespace Destiny.Maple.Shops
{
    public sealed class ShopItem
    {
        public Shop Parent { get; private set; }

        public int MapleID { get; private set; }
        public short Quantity { get; private set; }
        public int PurchasePrice { get; private set; }
        public int Sort { get; private set; }

        public short MaxPerStack
        {
            get
            {
                return DataProvider.Items[this.MapleID].MaxPerStack;
            }
        }

        public int SalePrice
        {
            get
            {
                return DataProvider.Items[this.MapleID].SalePrice;
            }
        }

        public double UnitPrice
        {
            get
            {
                return this.Parent.UnitPrices[this.MapleID];
            }
        }

        public bool IsRecharageable
        {
            get
            {
                return DataProvider.Items[this.MapleID].IsRechargeable;
            }
        }

        public ShopItem(Shop parent, Datum datum)
        {
            this.Parent = parent;

            this.MapleID = (int)datum["itemid"];
            this.Quantity = (short)datum["quantity"];
            this.PurchasePrice = (int)datum["price"];
            this.Sort = (int)datum["sort"];
        }

        public ShopItem(Shop parent, int mapleID)
        {
            this.Parent = parent;

            this.MapleID = mapleID;
            this.Quantity = 1;
            this.PurchasePrice = 0;
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteInt(this.MapleID)
                    .WriteInt(this.PurchasePrice)
                    .WriteInt() // NOTE: Perfect Pitch.
                    .WriteInt() // NOTE: Time limit.
                    .WriteInt(); // NOTE: Unknown.

                if (this.IsRecharageable)
                {
                    oPacket
                        .WriteShort()
                        .WriteInt()
                        .WriteShort((short)(BitConverter.DoubleToInt64Bits(this.UnitPrice) >> 48))
                        .WriteShort(this.MaxPerStack);
                }
                else
                {
                    oPacket
                        .WriteShort(this.Quantity)
                        .WriteShort(this.MaxPerStack);
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }
    }
}
