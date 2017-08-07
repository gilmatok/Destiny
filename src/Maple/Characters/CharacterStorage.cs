using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Life;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterStorage
    {
        public Character Parent { get; private set; }

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

        public void Load() // TODO: Refactor loading code, kinda hackish.
        {
            Datum datum = new Datum("storages");

            try
            {
                datum.Populate("AccountID = '{0}'", this.Parent.AccountID);
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

            foreach (Item item in this.Parent.Items.GetStored())
            {
                this.Items.Add(item);
            }
        }

        public void Save()
        {
            Datum datum = new Datum("storages");

            datum["Slots"] = this.Slots;
            datum["Meso"] = this.Meso;

            datum.Update("AccountID = '{0}'", this.Parent.AccountID);
        }

        public void Show(Npc npc)
        {
            this.Load();

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Storage))
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
                    item.Encode(oPacket, true, true);
                }

                oPacket
                    .WriteShort()
                    .WriteByte();

                this.Parent.Client.Send(oPacket);
            }
        }

        public void Handle(InPacket iPacket)
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
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Storage))
                            {
                                oPacket.WriteByte(17);

                                this.Parent.Client.Send(oPacket);
                            }

                            return;
                        }

                        if (this.Parent.Meso <= 500) // TODO: Actual storage cost.
                        {
                            this.Parent.Notify("You don't have enough meso to store the item.", NoticeType.Popup);

                            return;
                        }

                        if (item.Quantity != quantity)
                        {
                            this.Parent.Items.Remove(item.MapleID, quantity);
                        }
                        else
                        {
                            this.Parent.Items.Remove(item, true);
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

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Storage))
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
