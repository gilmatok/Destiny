using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Life;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterStorage
    {
        public Character Parent { get; private set; }

        public Npc Npc { get; private set; }
        public byte Slots { get; private set; }
        public int Meso { get; private set; }

        private List<Item> Items { get; set; }

        public bool IsFull
        {
            get
            {
                return this.Items.Count == this.Slots;
            }
        }

        public CharacterStorage(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            Datum datum = new Datum("storages");

            try
            {
                datum.Populate("AccountID = {0}", this.Parent.AccountID);
            }
            catch
            {
                datum["AccountID"] = this.Parent.AccountID;
                datum["Slots"] = (byte)4;
                datum["Meso"] = 0;

                datum.Insert();
            }

            this.Slots = (byte)datum["Slots"];
            this.Meso = (int)datum["Meso"];

            this.Items = new List<Item>();

            foreach (Datum itemDatum in new Datums("items").Populate("AccountID = {0} AND IsStored = True", this.Parent.AccountID))
            {
                this.Items.Add(new Item(itemDatum));
            }
        }

        public void Save()
        {
            Datum datum = new Datum("storages");

            datum["Slots"] = this.Slots;
            datum["Meso"] = this.Meso;

            datum.Update("AccountID = {0}", this.Parent.AccountID);

            foreach (Item item in this.Items)
            {
                item.Save();
            }
        }

        public void Show(Npc npc)
        {
            this.Npc = npc;

            this.Load();

            using (Packet oPacket = new Packet(ServerOperationCode.Storage))
            {
                oPacket
                    .WriteByte(22)
                    .WriteInt(npc.MapleID)
                    .WriteByte(this.Slots)
                    .WriteShort(126)
                    .WriteShort()
                    .WriteInt()
                    .WriteInt(this.Meso)
                    .WriteShort()
                    .WriteByte((byte)this.Items.Count);

                foreach (Item item in this.Items)
                {
                    oPacket.WriteBytes(item.ToByteArray(true, true));
                }

                oPacket
                    .WriteShort()
                    .WriteByte();

                this.Parent.Client.Send(oPacket);
            }
        }

        public void Handle(Packet iPacket)
        {
            StorageAction action = (StorageAction)iPacket.ReadByte();

            switch (action)
            {
                case StorageAction.Withdraw:
                    {
                        ItemType type = (ItemType)iPacket.ReadByte();
                        byte slot = iPacket.ReadByte();

                        Item item = this.Items[slot];

                        if (item == null)
                        {
                            return;
                        }

                        this.Items.Remove(item);
                        item.Delete();

                        item.IsStored = false;

                        this.Parent.Items.Add(item, forceGetSlot: true);

                        List<Item> itemsByType = new List<Item>();

                        foreach (Item loopItem in this.Items)
                        {
                            if (loopItem.Type == item.Type)
                            {
                                itemsByType.Add(loopItem);
                            }
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.Storage))
                        {
                            oPacket
                                .WriteByte(13)
                                .WriteByte(this.Slots)
                                .WriteShort((short)(2 << (byte)item.Type))
                                .WriteShort()
                                .WriteInt()
                                .WriteByte((byte)itemsByType.Count);

                            foreach (Item loopItem in itemsByType)
                            {
                                oPacket.WriteBytes(loopItem.ToByteArray(true, true));
                            }

                            this.Parent.Client.Send(oPacket);
                        }
                    }
                    break;

                case StorageAction.Deposit:
                    {
                        short slot = iPacket.ReadShort();
                        int itemID = iPacket.ReadInt();
                        short quantity = iPacket.ReadShort();

                        Item item = this.Parent.Items[itemID, slot];

                        if (this.IsFull)
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.Storage))
                            {
                                oPacket.WriteByte(17);

                                this.Parent.Client.Send(oPacket);
                            }

                            return;
                        }

                        if (this.Parent.Meso <= this.Npc.StorageCost)
                        {
                            this.Parent.Notify("You don't have enough meso to store the item.", NoticeType.Popup); // TOOD: Is there a packet for this?

                            return;
                        }

                        this.Parent.Meso -= this.Npc.StorageCost;

                        this.Parent.Items.Remove(item, true);

                        item.Parent = this.Parent.Items; // NOTE: This is needed because when we remove the item is sets parent to none.
                        item.Slot = (short)this.Items.Count;
                        item.IsStored = true;

                        this.Items.Add(item);

                        List<Item> itemsByType = new List<Item>();

                        foreach (Item loopItem in this.Items)
                        {
                            if (loopItem.Type == item.Type)
                            {
                                itemsByType.Add(loopItem);
                            }
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.Storage))
                        {
                            oPacket
                                .WriteByte(13)
                                .WriteByte(this.Slots)
                                .WriteShort((short)(2 << (byte)item.Type))
                                .WriteShort()
                                .WriteInt()
                                .WriteByte((byte)itemsByType.Count);

                            foreach (Item loopItem in itemsByType)
                            {
                                oPacket.WriteBytes(loopItem.ToByteArray(true, true));
                            }

                            this.Parent.Client.Send(oPacket);
                        }
                    }
                    break;

                case StorageAction.ModifyMeso:
                    {
                        int meso = iPacket.ReadInt();

                        if (meso > 0) // NOTE: Withdraw meso.
                        {
                            // TODO: Meso checks.
                        }
                        else // NOTE: Deposit meso.
                        {
                            // TODO: Meso checks.
                        }

                        this.Meso -= meso;
                        this.Parent.Meso += meso;

                        using (Packet oPacket = new Packet(ServerOperationCode.Storage))
                        {
                            oPacket
                                .WriteByte(19)
                                .WriteByte(this.Slots)
                                .WriteShort(2)
                                .WriteShort()
                                .WriteInt()
                                .WriteInt(this.Meso);

                            this.Parent.Client.Send(oPacket);
                        }
                    }
                    break;

                case StorageAction.Leave:
                    {
                        this.Save();
                    }
                    break;
            }
        }
    }
}
