using Destiny.Core.IO;
using Destiny.Utility;

// TODO: Consider refactoring this entire class. Things seem to get a little hacky.
namespace Destiny.Game
{
    public sealed class CharacterItems
    {
        public Character Parent { get; private set; }

        private Item[] mEquipped;
        private Item[] mCashEquipped;
        private Item[][] mItems;

        public CharacterItems(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;

            mEquipped = new Item[100];
            mCashEquipped = new Item[100];
            mItems = new Item[5][];

            for (int i = 0; i < 5; i++)
            {
                mItems[i] = new Item[24];
            }

            while (query.NextRow())
            {
                byte inventory = query.GetByte("inventory");

                if (inventory == 0)
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
                        mItems[0][equip.Slot] = equip;
                    }
                }
                else
                {
                    Item item = new Item(query);
                }
            }
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(24)
                .WriteByte(24)
                .WriteByte(24)
                .WriteByte(24)
                .WriteByte(48)
                .WriteLong();

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
    }
}
