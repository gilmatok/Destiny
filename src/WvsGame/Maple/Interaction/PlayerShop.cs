using Destiny.Maple.Maps;
using Destiny.Network;
using Destiny.Maple.Characters;
using System.Collections.Generic;

namespace Destiny.Maple.Interaction
{
    public sealed class PlayerShop : MapObject, ISpawnable
    {
        public Character Owner { get; private set; }
        public string Description { get; private set; }
        public Character[] Visitors { get; private set; }
        public List<PlayerShopItem> Items { get; private set; }
        public bool Opened { get; private set; }

        public bool IsFull
        {
            get
            {
                for (int i = 0; i < this.Visitors.Length; i++)
                {
                    if (this.Visitors[i] == null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public PlayerShop(Character owner, string description)
        {
            this.Owner = owner;
            this.Description = description;
            this.Visitors = new Character[3];
            this.Items = new List<PlayerShopItem>();
            this.Opened = false;

            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
            {
                oPacket
                    .WriteByte((byte)InteractionCode.Room)
                    .WriteByte(4)
                    .WriteByte(4)
                    .WriteByte(0)
                    .WriteByte(0)
                    .WriteBytes(this.Owner.AppearanceToByteArray())
                    .WriteString(this.Owner.Name)
                    .WriteByte(byte.MaxValue)
                    .WriteString(this.Description)
                    .WriteByte(16)
                    .WriteByte(0);

                this.Owner.Client.Send(oPacket);
            }
        }

        public void Handle(Character character, InteractionCode code, Packet iPacket)
        {
            switch (code)
            {
                case InteractionCode.OpenStore:
                    {
                        this.Owner.Map.PlayerShops.Add(this);

                        this.Opened = true;
                    }
                    break;

                case InteractionCode.AddItem:
                    {
                        ItemType type = (ItemType)iPacket.ReadByte();
                        short slot = iPacket.ReadShort();
                        short bundles = iPacket.ReadShort();
                        short perBundle = iPacket.ReadShort();
                        int price = iPacket.ReadInt();
                        short quantity = (short)(bundles * perBundle);

                        Item item = character.Items[type, slot];

                        if (item == null)
                        {
                            return;
                        }

                        if (perBundle < 0 || perBundle * bundles > 2000 || bundles < 0 || price < 0)
                        {
                            return;
                        }

                        if (quantity > item.Quantity)
                        {
                            return;
                        }

                        if (quantity < item.Quantity)
                        {
                            item.Quantity -= quantity;
                            item.Update();
                        }
                        else
                        {
                            character.Items.Remove(item, true);
                        }

                        PlayerShopItem shopItem = new PlayerShopItem(item.MapleID, bundles, quantity, price);

                        this.Items.Add(shopItem);

                        this.UpdateItems();
                    }
                    break;

                case InteractionCode.RemoveItem:
                    {
                        if (character == this.Owner)
                        {
                            short slot = iPacket.ReadShort();

                            PlayerShopItem shopItem = this.Items[slot];

                            if (shopItem == null)
                            {
                                return;
                            }

                            if (shopItem.Quantity > 0)
                            {
                                this.Owner.Items.Add(new Item(shopItem.MapleID, shopItem.Quantity));
                            }

                            this.Items.Remove(shopItem);

                            this.UpdateItems();
                        }
                    }
                    break;

                case InteractionCode.Exit:
                    {
                        if (character == this.Owner)
                        {
                            this.Close();
                        }
                        else
                        {
                            this.RemoveVisitor(character);
                        }
                    }
                    break;

                case InteractionCode.Buy:
                    {
                        short slot = iPacket.ReadByte();
                        short quantity = iPacket.ReadShort();

                        PlayerShopItem shopItem = this.Items[slot];

                        if (shopItem == null)
                        {
                            return;
                        }

                        if (character == this.Owner)
                        {
                            return;
                        }

                        if (quantity > shopItem.Quantity)
                        {
                            return;
                        }

                        if (character.Meso < shopItem.MerchantPrice * quantity)
                        {
                            return;
                        }

                        shopItem.Quantity -= quantity;

                        character.Meso -= shopItem.MerchantPrice * quantity;
                        this.Owner.Meso += shopItem.MerchantPrice * quantity;

                        character.Items.Add(new Item(shopItem.MapleID, quantity));

                        this.UpdateItems(); // TODO: This doesn't mark the item as sold.

                        bool noItemLeft = true;

                        foreach (PlayerShopItem loopShopItem in this.Items)
                        {
                            if (loopShopItem.Quantity > 0)
                            {
                                noItemLeft = false;

                                break;
                            }
                        }

                        if (noItemLeft)
                        {
                            // TODO: Close the owner's shop.
                            // TODO: Notify  the owner the shop has been closed due to items being sold out.

                            this.Close();
                        }
                    }
                    break;

                case InteractionCode.Chat:
                    {
                        string text = iPacket.ReadString();

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.Chat)
                                .WriteByte(8);

                            byte sender = 0;

                            for (int i = 0; i < this.Visitors.Length; i++)
                            {
                                if (this.Visitors[i] == character)
                                {
                                    sender = (byte)(i + 1);
                                }
                            }

                            oPacket
                                .WriteByte(sender)
                                .WriteString("{0} : {1}", character.Name, text);

                            this.Broadcast(oPacket);
                        }
                    }
                    break;
            }
        }

        public void Close()
        {
            foreach (PlayerShopItem loopShopItem in this.Items)
            {
                if (loopShopItem.Quantity > 0)
                {
                    this.Owner.Items.Add(new Item(loopShopItem.MapleID, loopShopItem.Quantity));
                }
            }

            if (this.Opened)
            {
                this.Map.PlayerShops.Remove(this);

                for (int i = 0; i < this.Visitors.Length; i++)
                {
                    if (this.Visitors[i] != null)
                    {
                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.Exit)
                                .WriteByte(1)
                                .WriteByte(10);

                            this.Visitors[i].Client.Send(oPacket);
                        }

                        this.Visitors[i].PlayerShop = null;
                    }
                }
            }

            this.Owner.PlayerShop = null;
        }

        public void UpdateItems()
        {
            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
            {
                oPacket
                    .WriteByte((byte)InteractionCode.UpdateItems)
                    .WriteByte((byte)this.Items.Count);

                foreach (PlayerShopItem loopShopItem in this.Items)
                {
                    oPacket
                        .WriteShort(loopShopItem.Bundles)
                        .WriteShort(loopShopItem.Quantity)
                        .WriteInt(loopShopItem.MerchantPrice)
                        .WriteBytes(loopShopItem.ToByteArray(true, true));
                }

                this.Broadcast(oPacket);
            }
        }

        public void Broadcast(Packet oPacket, bool includeOwner = true)
        {
            if (includeOwner)
            {
                this.Owner.Client.Send(oPacket);
            }

            for (int i = 0; i < this.Visitors.Length; i++)
            {
                if (this.Visitors[i] != null)
                {
                    this.Visitors[i].Client.Send(oPacket);
                }
            }
        }

        public void AddVisitor(Character visitor)
        {
            for (int i = 0; i < this.Visitors.Length; i++)
            {
                if (this.Visitors[i] == null)
                {
                    using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                    {
                        oPacket
                            .WriteByte((byte)InteractionCode.Visit)
                            .WriteByte((byte)(i + 1))
                            .WriteBytes(visitor.AppearanceToByteArray())
                            .WriteString(visitor.Name);

                        this.Broadcast(oPacket);
                    }

                    visitor.PlayerShop = this;
                    this.Visitors[i] = visitor;

                    using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                    {
                        oPacket
                            .WriteByte((byte)InteractionCode.Room)
                            .WriteByte(4)
                            .WriteByte(4)
                            .WriteBool(true)
                            .WriteByte(0)
                            .WriteBytes(this.Owner.AppearanceToByteArray())
                            .WriteString(this.Owner.Name);

                        for (int slot = 0; slot < 3; slot++)
                        {
                            if (this.Visitors[slot] != null)
                            {
                                oPacket
                                    .WriteByte((byte)(slot + 1))
                                    .WriteBytes(this.Visitors[slot].AppearanceToByteArray())
                                    .WriteString(this.Visitors[slot].Name);
                            }
                        }

                        oPacket
                            .WriteByte(byte.MaxValue)
                            .WriteString(this.Description)
                            .WriteByte(0x10)
                            .WriteByte((byte)this.Items.Count);

                        foreach (PlayerShopItem loopShopItem in this.Items)
                        {
                            oPacket
                                .WriteShort(loopShopItem.Bundles)
                                .WriteShort(loopShopItem.Quantity)
                                .WriteInt(loopShopItem.MerchantPrice)
                                .WriteBytes(loopShopItem.ToByteArray(true, true));
                        }

                        visitor.Client.Send(oPacket);
                    }

                    break;
                }
            }
        }

        public void RemoveVisitor(Character visitor)
        {
            for (int i = 0; i < this.Visitors.Length; i++)
            {
                if (this.Visitors[i] == visitor)
                {
                    visitor.PlayerShop = null;
                    this.Visitors[i] = null;

                    using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                    {
                        oPacket.WriteByte((byte)InteractionCode.Exit);

                        if (i > 0)
                        {
                            oPacket.WriteByte((byte)(i + 1));
                        }

                        this.Broadcast(oPacket, false);
                    }

                    using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                    {
                        oPacket
                            .WriteByte((byte)InteractionCode.Exit)
                            .WriteByte((byte)(i + 1));

                        this.Owner.Client.Send(oPacket);
                    }

                    break;
                }
            }
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.AnnounceBox);

            oPacket
                .WriteInt(this.Owner.ID)
                .WriteByte(4)
                .WriteInt(this.ObjectID)
                .WriteString(this.Description)
                .WriteByte(0)
                .WriteByte(0)
                .WriteByte(1)
                .WriteByte(4)
                .WriteByte(0);

            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.AnnounceBox);

            oPacket
                .WriteInt(this.Owner.ID)
                .WriteByte(0);

            return oPacket;
        }
    }
}
