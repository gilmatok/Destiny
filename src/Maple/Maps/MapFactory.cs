using System.Collections.ObjectModel;

namespace Destiny.Maple.Maps
{
    public sealed class MapFactory : KeyedCollection<int, Map>
    {
        public MapFactory() : base() { }

        public new Map this[int key]
        {
            get
            {
                if (!base.Contains(key))
                {
                    this.Add(new Map(this, key));
                }

                return base[key];
            }
        }

        protected override int GetKeyForItem(Map item)
        {
            return item.MapleID;
        }
    }
}
