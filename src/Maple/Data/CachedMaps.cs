using Destiny.Data;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using Destiny.Maple.Shops;
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
            }

            using (Log.Load("Seats"))
            {
                foreach (Datum datum in new Datums("map_seats").Populate())
                {
                    this[(int)datum["mapid"]].Seats.Add(new Seat(datum));
                }
            }

            using (Log.Load("Portals"))
            {
                foreach (Datum datum in new Datums("map_portals").Populate())
                {
                    this[(int)datum["mapid"]].Portals.Add(new Portal(datum));
                }
            }

            using (Log.Load("Life"))
            {
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

            using (Log.Load("Shops"))
            {
                foreach (Datum datum in new Datums("shop_data").Populate())
                {
                    foreach (Map map in this)
                    {
                        foreach (Npc npc in map.Npcs)
                        {
                            if (npc.MapleID == (int)datum["npcid"])
                            {
                                npc.Shop = new Shop(npc, datum);
                            }
                        }
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
