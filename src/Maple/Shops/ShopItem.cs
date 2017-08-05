using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Data;

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
                return DataProvider.CachedItems[this.MapleID].MaxPerStack;
            }
        }

        public int SalePrice
        {
            get
            {
                return DataProvider.CachedItems[this.MapleID].SalePrice;
            }
        }

        public bool IsRecharageable
        {
            get
            {
                return DataProvider.CachedItems[this.MapleID].IsRechargeable;
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

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.MapleID)
                .WriteInt(this.PurchasePrice);

            if (this.IsRecharageable)
            {

            }
            else
            {
                oPacket
                    .WriteShort(this.Quantity)
                    .WriteShort(this.MaxPerStack);
            }
        }
    }
}
