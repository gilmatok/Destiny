using Destiny.Data;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedMaps : KeyedCollection<int, Map>
    {
        public CachedMaps()
            : base()
        {
            using (Log.Load("Maps"))
            {
                foreach (Datum datum in new Datums("map_data").Populate())
                {
                    this.Add(new Map(datum));
                }

                foreach (Datum datum in new Datums("map_portals").Populate())
                {
                    this[(int)datum["mapid"]].Portals.Add(new Portal(datum));
                }

                foreach (Datum datum in new Datums("map_life").Populate())
                {
                    switch ((string)datum["life_type"])
                    {
                        case "npc":
                            this[(int)datum["mapid"]].Npcs.Add(new Npc(datum));
                            break;

                        case "mob":
                            this[(int)datum["mapid"]].SpawnPoints.Add(new SpawnPoint(datum));
                            break;
                    }
                }
            }
        }

        protected override int GetKeyForItem(Map item)
        {
            return item.MapleID;
        }
    }
}
