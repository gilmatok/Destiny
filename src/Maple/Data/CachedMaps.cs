using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using Destiny.Utility;
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
                using (DatabaseQuery query = Database.Query("SELECT * FROM map_data"))
                {
                    while (query.NextRow())
                    {
                        this.Add(new Map(query));
                    }
                }

                using (DatabaseQuery query = Database.Query("SELECT * FROM map_portals"))
                {
                    while (query.NextRow())
                    {
                        this[query.GetInt("mapid")].Portals.Add(new Portal(query));
                    }
                }

                using (DatabaseQuery query = Database.Query("SELECT * FROM map_life"))
                {
                    while (query.NextRow())
                    {
                        switch (query.GetString("life_type"))
                        {
                            case "npc":
                                this[query.GetInt("mapid")].Npcs.Add(new Npc(query));
                                break;

                            case "mob":
                                this[query.GetInt("mapid")].SpawnPoints.Add(new SpawnPoint(query));
                                break;
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
