using Destiny.Core.IO;
using Destiny.Utility;
using System;

// TODO: Consider refactoring this entire class. Things seem to get a little hacky.
namespace Destiny.Game
{
    public sealed class CharacterItems
    {
        public Character Parent { get; private set; }

        private Item[] mEquipped;
        private Item[] mCashEquipped;
        private Item[][] mItems;

        public CharacterItems(Character parent, byte[] slots, DatabaseQuery query)
        {
            this.Parent = parent;

            mEquipped = new Item[(byte)EquipmentSlot.Count];
            mCashEquipped = new Item[(byte)EquipmentSlot.Count];
            mItems = new Item[slots.Length][];

            for (int i = 0; i < slots.Length; i++)
            {
                mItems[i] = new Item[slots[i]];
            }

            while (query.NextRow())
            {
                if (query.GetByte("inventory") == 0)
                {
                    Item equip = new Equip(query);

                    if (equip.Slot < 0)
                    {
                        if (equip.Slot < -100)
                        {
                            mCashEquipped[(-equip.Slot) - 100] = equip;
                        }
                        else
                        {
                            mEquipped[-equip.Slot] = equip;
                        }
                    }
                    else
                    {
                        mItems[(byte)InventoryType.Equipment][equip.Slot] = equip;
                    }
                }
                else
                {
                    Item item = new Item(query);

                    mItems[query.GetByte("inventory")][item.Slot] = item;
                }
            }
        }

        public void Encode(OutPacket oPacket)
        {
            Array.ForEach(mItems, m => oPacket.WriteByte((byte)m.Length));

            oPacket.WriteLong();

            for (short i = 0; i < mEquipped.Length; i++)
            {
                if (mEquipped[i] != null)
                {
                    oPacket.WriteShort(i);
                    mEquipped[i].Encode(oPacket);
                }
            }
            oPacket.WriteShort();

            for (short i = 0; i < mCashEquipped.Length; i++)
            {
                if (mCashEquipped[i] != null)
                {
                    oPacket.WriteShort(i);
                    mCashEquipped[i].Encode(oPacket);
                }
            }
            oPacket.WriteShort();

            for (short i = 0; i < mItems[(byte)InventoryType.Equipment].Length; i++)
            {
                if (mItems[(byte)InventoryType.Equipment][i] != null)
                {
                    oPacket.WriteShort(i);
                    mItems[(byte)InventoryType.Equipment][i].Encode(oPacket);
                }
            }
            oPacket.WriteShort();

            // TODO: Evan inventory (they exist in v0.83, it seems).
            oPacket.WriteShort();

            for (byte i = 0; i < mItems[(byte)InventoryType.Usable].Length; i++)
            {
                if (mItems[(byte)InventoryType.Usable][i] != null)
                {
                    oPacket.WriteByte(i);
                    mItems[(byte)InventoryType.Usable][i].Encode(oPacket);
                }
            }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Setup].Length; i++)
            {
                if (mItems[(byte)InventoryType.Setup][i] != null)
                {
                    oPacket.WriteByte(i);
                    mItems[(byte)InventoryType.Setup][i].Encode(oPacket);
                }
            }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Etcetera].Length; i++)
            {
                if (mItems[(byte)InventoryType.Etcetera][i] != null)
                {
                    oPacket.WriteByte(i);
                    mItems[(byte)InventoryType.Etcetera][i].Encode(oPacket);
                }
            }
            oPacket.WriteByte();

            for (byte i = 0; i < mItems[(byte)InventoryType.Cash].Length; i++)
            {
                if (mItems[(byte)InventoryType.Cash][i] != null)
                {
                    oPacket.WriteByte(i);
                    mItems[(byte)InventoryType.Cash][i].Encode(oPacket);
                }
            }
            oPacket.WriteByte();
        }

        public void EncodeEquipment(OutPacket oPacket)
        {
            for (byte i = 0; i < (byte)EquipmentSlot.Count; i++)
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

            oPacket.WriteInt(mCashEquipped[(byte)EquipmentSlot.Weapon] == null ? 0 : mCashEquipped[(byte)EquipmentSlot.Weapon].MapleID);
        }
    }
}
