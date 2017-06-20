using Destiny.Utility;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedItems : KeyedCollection<int, Item>
    {
        public CachedItems()
            : base()
        {
            using (Log.Load("Items"))
            {
                using (DatabaseQuery query = Database.Query("SELECT * FROM item_data"))
                {
                    while (query.NextRow())
                    {
                        this.Add(new Item(query));
                    }
                }
            }
        }

        protected override int GetKeyForItem(Item item)
        {
            return item.MapleID;
        }
    }
}
