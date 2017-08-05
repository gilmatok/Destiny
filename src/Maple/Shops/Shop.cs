using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using System.Collections.Generic;

namespace Destiny.Maple.Shops
{
    public sealed class Shop
    {
        public int ID { get; private set; }
        public Npc Parent { get; private set; }
        public List<ShopItem> Items { get; private set; }

        private byte RechargeTierID { get; set; }

        public Shop(Npc parent, Datum datum)
        {
            this.Parent = parent;

            this.ID = (int)datum["shopid"];
            this.RechargeTierID = (byte)(int)datum["recharge_tier"];

            this.Items = new List<ShopItem>();

            foreach (Datum itemDatum in new Datums("shop_items").Populate("shopid = '{0}'", this.ID))
            {
                this.Items.Add(new ShopItem(this, itemDatum));
            }

            if (this.RechargeTierID > 0)
            {

            }
        }

        public void Show(Character customer)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.OpenNpcShop))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteShort((short)this.Items.Count);

                foreach (ShopItem loopShopItem in this.Items)
                {
                    loopShopItem.Encode(oPacket);
                }

                customer.Client.Send(oPacket);
            }
        }

        public void Handle(Character customer, InPacket iPacket)
        {
            ShopAction action = (ShopAction)iPacket.ReadByte();

            switch (action)
            {
                case ShopAction.Buy:
                    {
                        short index = iPacket.ReadShort();
                        int mapleID = iPacket.ReadInt();
                        short quantity = iPacket.ReadShort();

                        ShopItem item = this.Items[index];

                        if (customer.Meso < item.PurchasePrice * quantity)
                        {
                            return;
                        }

                        Item purchase;
                        int price;

                        if (item.IsRecharageable)
                        {
                            purchase = new Item(item.MapleID, item.MaxPerStack);
                            price = item.PurchasePrice;
                        }
                        else if (item.Quantity > 1)
                        {
                            purchase = new Item(item.MapleID, item.Quantity);
                            price = item.PurchasePrice;
                        }
                        else
                        {
                            purchase = new Item(item.MapleID, quantity);
                            price = item.PurchasePrice * quantity;
                        }

                        if (customer.Items.SpaceTakenBy(purchase) > customer.Items.RemainingSlots(purchase.Type))
                        {
                            customer.Notify("Your inventory is full.", NoticeType.Popup);
                        }
                        else
                        {
                            customer.Meso -= price;
                            customer.Items.Add(purchase);
                        }

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.ConfirmShopTransaction))
                        {
                            oPacket.WriteByte();

                            customer.Client.Send(oPacket);
                        }
                    }
                    break;

                case ShopAction.Sell:
                    {

                    }
                    break;

                case ShopAction.Recharge:
                    {

                    }
                    break;

                case ShopAction.Leave:
                    {

                    }
                    break;
            }
        }
    }
}
