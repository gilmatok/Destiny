using Destiny.Data;
using Destiny.IO;
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

            }
        }

        public new Map this[int key]
        {
            get
            {
                if (!base.Contains(key))
                {
                    using (Database.TemporarySchema(Database.SchemaMCDB))
                    {
                        foreach (Datum datum in new Datums("map_data", Database.SchemaMCDB).Populate("mapid = {0}", key))
                        {
                            this.Add(new Map(datum));
                        }

                        foreach (Datum datum in new Datums("map_footholds", Database.SchemaMCDB).Populate("mapid = {0}", key))
                        {
                            this[key].Footholds.Add(new Foothold(datum));
                        }

                        foreach (Datum datum in new Datums("map_seats", Database.SchemaMCDB).Populate("mapid = {0}", key))
                        {
                            this[key].Seats.Add(new Seat(datum));
                        }

                        foreach (Datum datum in new Datums("map_portals", Database.SchemaMCDB).Populate("mapid = {0}", key))
                        {
                            this[key].Portals.Add(new Portal(datum));
                        }

                        foreach (Datum datum in new Datums("map_life", Database.SchemaMCDB).Populate("mapid = {0}", key))
                        {
                            switch ((string)datum["life_type"])
                            {
                                case "npc":
                                    {
                                        this[key].Npcs.Add(new Npc(datum));
                                    }
                                    break;

                                case "mob":
                                    this[key].SpawnPoints.Add(new SpawnPoint(datum, true));
                                    break;

                                case "reactor":
                                    this[key].SpawnPoints.Add(new SpawnPoint(datum, false));
                                    break;
                            }
                        }
                    }

                    this[key].SpawnPoints.Spawn();
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
