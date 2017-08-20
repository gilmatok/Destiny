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

            using (Log.Load("Footholds"))
            {
                foreach (Datum datum in new Datums("map_footholds").Populate())
                {
                    this[(int)datum["mapid"]].Footholds.Add(new Foothold(datum));
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
                            this[(int)datum["mapid"]].SpawnPoints.Add(new SpawnPoint(datum, true));
                            break;

                        case "reactor":
                            this[(int)datum["mapid"]].SpawnPoints.Add(new SpawnPoint(datum, false));
                            break;
                    }
                }
            }

            using (Log.Load("Shops"))
            {
                Shop.LoadRechargeTiers();

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
