using Destiny.Core.IO;
using Destiny.Network.Handler;
using Destiny.Network.Packet;
using Destiny.Utility;
using System;

namespace Destiny.Game.Characters
{
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

            for (byte i = 0; i < slots.Length; i++)
            {
                mItems[i] = new Item[slots[i]];
            }

            while (query.NextRow())
            {
                short slot = query.GetShort("slot");

                if (slot < 0)
                {
                    if (slot < -100)
                    {
                        mCashEquipped[(-slot) - 100] = new Equip(query);
                    }
                    else
                    {
                        mEquipped[-slot] = new Equip(query);
                    }
                }
                else
                {
                    byte inventory = query.GetByte("inventory");

                    if (inventory == (byte)InventoryType.Equipment)
                    {
                        mItems[inventory][slot] = new Equip(query);
                    }
                    else
                    {
                        mItems[inventory][slot] = new Item(query);
                    }
                }
            }
        }

        public void Add(int mapleID, short quantity = 1)
        {
            Item item;

            InventoryType inventory = Item.GetInventory(mapleID);

            if (inventory == InventoryType.Equipment)
            {
                item = new Equip(mapleID);
            }
            else
            {
                item = new Item(mapleID, quantity);
            }

            short slot = this.GetNextFreeSlot(inventory);

            this[inventory, slot] = item;

            InventoryOperation operation = new InventoryOperation(InventoryOperationType.AddItem, item, 0, slot);

            this.Parent.Client.Send(InventoryPacket.InventoryOperation(false, operation));
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

                    this.Parent.Client.Send(InventoryPacket.InventoryOperation(true, operation));
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
            Array.ForEach(mItems, i => oPacket.WriteByte((byte)i.Length));

            oPacket.WriteLong(); // NOTE: Unknown.

            for (short i = 0; i < mEquipped.Length; i++) { if (mEquipped[i] != null) { oPacket.WriteShort(i); mEquipped[i].Encode(oPacket); } }
            oPacket.WriteShort();

            for (short i = 0; i < mCashEquipped.Length; i++) { if (mCashEquipped[i] != null) { oPacket.WriteShort(i); mCashEquipped[i].Encode(oPacket); } }
            oPacket.WriteShort();

            for (short i = 0; i < mItems[(byte)InventoryType.Equipment].Length; i++) { if (mItems[(byte)InventoryType.Equipment][i] != null) { oPacket.WriteShort(i); mItems[(byte)InventoryType.Equipment][i].Encode(oPacket); } }
            oPacket.WriteShort();

            // TODO: Evan inventory.
            oPacket.WriteShort();

            for (byte i = 0; i < mItems[(byte)InventoryType.Usable].Length; i++) { if (mItems[(byte)InventoryType.Usable][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Usable][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Setup].Length; i++) { if (mItems[(byte)InventoryType.Setup][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Setup][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Etcetera].Length; i++) { if (mItems[(byte)InventoryType.Etcetera][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Etcetera][i].Encode(oPacket); } }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Cash].Length; i++) { if (mItems[(byte)InventoryType.Cash][i] != null) { oPacket.WriteByte(i); mItems[(byte)InventoryType.Cash][i].Encode(oPacket); } }
            oPacket.WriteByte();
        }

        public void EncodeEquipment(OutPacket oPacket)
        {
            /*for (byte i = 0; i < (byte)EquipmentSlot.Count; i++)
            {
                if (mEquipped[i] == null && mCashEquipped[i] == null)
                {
                    continue;
                }

                if (i == (byte)EquipmentSlot.Weapon && mEquipped[i] != null)
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

            for (byte i = 0; i < (byte)EquipmentSlot.Count; i++)
            {
                if (i == (byte)EquipmentSlot.Weapon || mEquipped[i] == null || mCashEquipped[i] == null)
                {
                    continue;
                }

                oPacket.WriteInt(mEquipped[i].MapleID);
            }
            oPacket.WriteByte(byte.MaxValue);

            oPacket.WriteInt(mCashEquipped[(byte)EquipmentSlot.Weapon] == null ? 0 : mCashEquipped[(byte)EquipmentSlot.Weapon].MapleID);*/
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
    }
}
