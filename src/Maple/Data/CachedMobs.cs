using Destiny.Maple.Life;
using System.Collections.ObjectModel;
using Destiny.Core.Data;

namespace Destiny.Maple.Data
{
    public sealed class CachedMobs : KeyedCollection<int, Mob>
    {
        public CachedMobs()
            : base()
        {
            using (Log.Load("Mobs"))
            {
                foreach (Datum datum in new Datums("mob_data").Populate())
                {
                    this.Add(new Mob(datum));
                }
            }

            using (Log.Load("Loots"))
            {
                foreach (Datum datum in new Datums("drop_data").Populate())
                {
                    int dropperID = (int)datum["dropperid"];

                    if (this.Contains(dropperID))
                    {
                        this[dropperID].Loots.Add(new Loot(datum));
                    }
                }
            }
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.MapleID;
        }
    }
}
