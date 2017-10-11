using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using System.Collections.Generic;

namespace Destiny.Maple.Shops
{
    public sealed class Shop
    {
        public static void LoadRechargeTiers()
        {
            Shop.RechargeTiers = new Dictionary<byte, Dictionary<int, double>>();

            foreach (Datum datum in new Datums("shop_recharge_data").Populate())
            {
                if (!Shop.RechargeTiers.ContainsKey((byte)(int)datum["tierid"]))
                {
                    Shop.RechargeTiers.Add((byte)(int)datum["tierid"], new Dictionary<int, double>());
                }

                Shop.RechargeTiers[(byte)(int)datum["tierid"]].Add((int)datum["itemid"], (double)datum["price"]);
            }
        }

        private byte RechargeTierID { get; set; }

        public int ID { get; private set; }
        public Npc Parent { get; private set; }
        public List<ShopItem> Items { get; private set; }
        public static Dictionary<byte, Dictionary<int, double>> RechargeTiers { get; set; }

        public Dictionary<int, double> UnitPrices
        {
            get
            {
                return Shop.RechargeTiers[this.RechargeTierID];
            }
        }

        public Shop(Npc parent, Datum datum)
        {
            this.Parent = parent;

            this.ID = (int)datum["shopid"];
            this.RechargeTierID = (byte)(int)datum["recharge_tier"];

            this.Items = new List<ShopItem>();

            foreach (Datum itemDatum in new Datums("shop_items").Populate("shopid = {0} ORDER BY sort DESC", this.ID))
            {
                this.Items.Add(new ShopItem(this, itemDatum));
            }

            if (this.RechargeTierID > 0)
            {
                foreach (KeyValuePair<int, double> rechargeable in this.UnitPrices)
                {
                    this.Items.Add(new ShopItem(this, rechargeable.Key));
                }
            }
        }

        public void Show(Character customer)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.OpenNpcShop))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteShort((short)this.Items.Count);

                foreach (ShopItem loopShopItem in this.Items)
                {
                    oPacket.WriteBytes(loopShopItem.ToByteArray());
                }

                customer.Client.Send(oPacket);
            }
        }

        public void Handle(Character customer, Packet iPacket)
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

                        using (Packet oPacket = new Packet(ServerOperationCode.ConfirmShopTransaction))
                        {
                            oPacket.WriteByte();

                            customer.Client.Send(oPacket);
                        }
                    }
                    break;

                case ShopAction.Sell:
                    {
                        short slot = iPacket.ReadShort();
                        int mapleID = iPacket.ReadInt();
                        short quantity = iPacket.ReadShort();

                        Item item = customer.Items[mapleID, slot];

                        if (item.IsRechargeable)
                        {
                            quantity = item.Quantity;
                        }

                        if (quantity > item.Quantity)
                        {
                            return;
                        }
                        else if (quantity == item.Quantity)
                        {
                            customer.Items.Remove(item, true);
                        }
                        else if (quantity < item.Quantity)
                        {
                            item.Quantity -= quantity;
                            item.Update();
                        }

                        if (item.IsRechargeable)
                        {
                            customer.Meso += item.SalePrice + (int)(this.UnitPrices[item.MapleID] * item.Quantity);
                        }
                        else
                        {
                            customer.Meso += item.SalePrice * quantity;
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.ConfirmShopTransaction))
                        {
                            oPacket.WriteByte(8);

                            customer.Client.Send(oPacket);
                        }
                    }
                    break;

                case ShopAction.Recharge:
                    {
                        short slot = iPacket.ReadShort();

                        Item item = customer.Items[ItemType.Usable, slot];

                        int price = (int)(this.UnitPrices[item.MapleID] * (item.MaxPerStack - item.Quantity));

                        if (customer.Meso < price)
                        {
                            customer.Notify("You do not have enough mesos.", NoticeType.Popup);
                        }
                        else
                        {
                            customer.Meso -= price;

                            item.Quantity = item.MaxPerStack;
                            item.Update();
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.ConfirmShopTransaction))
                        {
                            oPacket.WriteByte(8);

                            customer.Client.Send(oPacket);
                        }
                    }
                    break;

                case ShopAction.Leave:
                    {
                        customer.LastNpc = null;
                    }
                    break;
            }
        }
    }
}
