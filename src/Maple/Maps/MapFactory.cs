using System.Collections.ObjectModel;

namespace Destiny.Maple.Maps
{
    public sealed class MapFactory : KeyedCollection<int, Map>
    {
        public new Map this[int key]
        {
            get
            {
                if (!base.Contains(key))
                {
                    Map map = new Map(this, key);

                    map.SpawnPoints.Spawn();

                    this.Add(map);
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
