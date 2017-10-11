using Destiny.Data;
using Destiny.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedItems : KeyedCollection<int, Item>
    {
        public List<int> WizetItemIDs { get; private set; }

        public CachedItems()
            : base()
        {
            using (Log.Load("Items"))
            {
                foreach (Datum datum in new Datums("item_data").Populate())
                {
                    this.Add(new Item(datum));
                }
            }

            using (Log.Load("Consumables"))
            {
                foreach (Datum datum in new Datums("item_consume_data").Populate())
                {
                    this[(int)datum["itemid"]].LoadConsumeData(datum);
                }
            }

            using (Log.Load("Equips"))
            {
                foreach (Datum datum in new Datums("item_equip_data").Populate())
                {
                    this[(int)datum["itemid"]].LoadEquipmentData(datum);
                }
            }

            using (Log.Load("Summons"))
            {
                foreach (Datum datum in new Datums("item_summons").Populate())
                {
                    this[(int)datum["itemid"]].Summons.Add(new Tuple<int, short>((int)datum["mobid"], (short)datum["chance"]));
                }
            }

            this.WizetItemIDs = new List<int>(4);

            this.WizetItemIDs.Add(1002140);
            this.WizetItemIDs.Add(1322013);
            this.WizetItemIDs.Add(1042003);
            this.WizetItemIDs.Add(1062007);
        }

        protected override int GetKeyForItem(Item item)
        {
            return item.MapleID;
        }
    }
}
