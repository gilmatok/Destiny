using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Utility;

namespace Destiny.Maple.Characters
{
    public enum InventoryOperationType : byte
    {
        AddItem,
        ModifyQuantity,
        ModifySlot,
        RemoveItem
    }

    public sealed class InventoryOperation
    {
        public InventoryOperationType Type { get; private set; }
        public Item Item { get; private set; }
        public short OldSlot { get; private set; }
        public short CurrentSlot { get; private set; }

        public InventoryOperation(InventoryOperationType type, Item item, short oldSlot, short currentSlot)
        {
            this.Type = type;
            this.Item = item;
            this.OldSlot = oldSlot;
            this.CurrentSlot = currentSlot;
        }
    }

    public sealed class CharacterItems
    {
        public Character Parent { get; private set; }

        private Item[] mEquipped;
        private Item[] mCashEquipped;
        private Item[][] mItems;

        public CharacterItems(Character parent, byte[] slots, DatabaseQuery query)
            : base()
        {
            this.Parent = parent;

            mEquipped = new Item[51];
            mCashEquipped = new Item[51];
            mItems = new Item[(byte)InventoryType.Count][];

            for (byte i = 1; i < slots.Length; i++)
            {
                mItems[(byte)i] = new Item[slots[i]];
            }

            while (query.NextRow())
            {
                byte inventory = query.GetByte("inventory");
                short slot = query.GetShort("slot");

                Item item = new Item(query);

                if (slot < 0)
                {
                    if (slot < -100)
                    {
                        mCashEquipped[(-slot) - 100] = item;
                    }
                    else
                    {
                        mEquipped[-slot] = item;
                    }
                }
                else
                {
                    mItems[inventory][slot] = item;
                }
            }
        }

        public void Save()
        {

        }

        public void Add(int mapleID, short quantity = 1)
        {
            //InventoryType inventory = Item.GetInventory(mapleID);
            //short slot = this.GetNextFreeSlot(inventory);

            //Item item;

            //if (inventory == InventoryType.Equipment)
            //{
            //    item = new Equip(mapleID);
            //}
            //else
            //{
            //    item = new Item(mapleID, quantity);
            //}

            //this[inventory, slot] = item;

            //InventoryOperation operation = new InventoryOperation(InventoryOperationType.AddItem, item, 0, slot);

            //this.Operate(false, operation);
        }

        public void Swap(InventoryType inventory, short slot1, short slot2)
        {
            bool equippedSlot2 = slot2 < 0;

            if (inventory == InventoryType.Equipment && equippedSlot2)
            {

            }
            else
            {
                Item item1 = this[inventory, slot1];
                Item item2 = this[inventory, slot2];

                if (item1 == null)
                {
                    return;
                }

                if (item2 != null && item1.MapleID == item2.MapleID)
                {

                }
                else
                {
                    this[inventory, slot1] = item2;
                    this[inventory, slot2] = item1;

                    InventoryOperation operation = new InventoryOperation(InventoryOperationType.ModifySlot, item1, slot1, slot2);

                    this.Operate(true, operation);
                }
            }
        }

        private short GetNextFreeSlot(InventoryType inventory)
        {
            for (short i = 1; i < mItems[(byte)inventory].Length; i++)
            {
                if (mItems[(byte)inventory][i] == null)
                {
                    return i;
                }
            }

            throw new InventoryFullException();
        }

        // TODO: Beautify this.
        public void Encode(OutPacket oPacket)
        {
            for (byte i = 1; i < mItems.Length; i++) oPacket.WriteByte((byte)mItems[i].Length);

            oPacket.WriteLong(); // NOTE: Unknown.

            for (short i = 1; i < mEquipped.Length; i++) { if (mEquipped[i] != null) { oPacket.WriteShort(i); mEquipped[i].Encode(oPacket); } }
            oPacket.WriteShort();

            for (short i = 1; i < mCashEquipped.Length; i++) { if (mCashEquipped[i] != null) { oPacket.WriteShort(i); mCashEquipped[i].Encode(oPacket); } }
            oPacket.WriteShort();

            for (short i = 1; i < mItems[(byte)InventoryType.Equipment].Length; i++) { if (mItems[(byte)InventoryType.Equipment][i] != null) { oPacket.WriteShort(i); mItems[(byte)InventoryType.Equipment][i].Encode(oPacket); } }
            oPacket.WriteShort();

            // TODO: Evan inventory.
            oPacket.WriteShort();

            for (byte i = 1; i < mItems[(byte)InventoryType.Usable].Length; i++) { if (mItems[(byte)InventoryType.Usable][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Usable][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 1; i < mItems[(byte)InventoryType.Setup].Length; i++) { if (mItems[(byte)InventoryType.Setup][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Setup][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 1; i < mItems[(byte)InventoryType.Etcetera].Length; i++) { if (mItems[(byte)InventoryType.Etcetera][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Etcetera][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 1; i < mItems[(byte)InventoryType.Cash].Length; i++) { if (mItems[(byte)InventoryType.Cash][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Cash][i].Encode(oPacket); } }
            oPacket.WriteByte();
        }

        public void EncodeEquipment(OutPacket oPacket)
        {
            for (byte i = 0; i < 51; i++)
            {
                if (mEquipped[i] == null && mCashEquipped[i] == null)
                {
                    continue;
                }

                oPacket.WriteByte(i);

                if (i == 11 && mEquipped[i] != null)
                {
                    oPacket.WriteInt(mEquipped[i].MapleID);
                }
                else if (mCashEquipped[i] != null)
                {
                    oPacket.WriteInt(mCashEquipped[i].MapleID);
                }
                else if (mEquipped[i] != null)
                {
                    oPacket.WriteInt(mEquipped[i].MapleID);
                }
            }
            oPacket.WriteByte(byte.MaxValue);

            for (byte i = 0; i < 51; i++)
            {
                if (i == 11 || mEquipped[i] == null || mCashEquipped[i] == null)
                {
                    continue;
                }

                oPacket.WriteByte(i);
                oPacket.WriteInt(mEquipped[i].MapleID);
            }
            oPacket.WriteByte(byte.MaxValue);

            oPacket.WriteInt(mCashEquipped[11] == null ? 0 : mCashEquipped[11].MapleID);
        }

        public Item this[InventoryType inventory, short slot]
        {
            get
            {
                return mItems[(byte)inventory][slot];
            }
            set
            {
                mItems[(byte)inventory][slot] = value;
            }
        }

        private void Operate(bool unk, params InventoryOperation[] operations)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.InventoryOperation))
            {
                oPacket
                    .WriteBool(unk)
                    .WriteByte((byte)operations.Length);

                sbyte addedByte = -1;

                foreach (InventoryOperation operation in operations)
                {
                    oPacket
                        .WriteByte((byte)operation.Type)
                        .WriteByte((byte)operation.Item.Type);

                    switch (operation.Type)
                    {
                        case InventoryOperationType.AddItem:
                            {
                                oPacket.WriteShort(operation.CurrentSlot);
                                operation.Item.Encode(oPacket);
                            }
                            break;

                        case InventoryOperationType.ModifyQuantity:
                            {
                                oPacket
                                    .WriteShort(operation.CurrentSlot)
                                    .WriteShort(operation.Item.Quantity);
                            }
                            break;

                        case InventoryOperationType.ModifySlot:
                            {
                                oPacket
                                    .WriteShort(operation.OldSlot)
                                    .WriteShort(operation.CurrentSlot);

                                if (addedByte == -1)
                                {
                                    if (operation.OldSlot < 0)
                                    {
                                        addedByte = 1;
                                    }
                                    else if (operation.CurrentSlot < 0)
                                    {
                                        addedByte = 2;
                                    }
                                }
                            }
                            break;

                        case InventoryOperationType.RemoveItem:
                            {
                                oPacket.WriteShort(operation.CurrentSlot);
                            }
                            break;
                    }
                }

                if (addedByte != -1)
                {
                    oPacket.WriteSByte(addedByte);
                }

                this.Parent.Client.Send(oPacket);
            }
        }
    }
}
