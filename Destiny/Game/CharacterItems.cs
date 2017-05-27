using Destiny.Core.IO;
using Destiny.Utility;

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

                if (inventory == 1)
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

        }
    }
}
