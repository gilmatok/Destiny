using Destiny.Maple.Life;
using Destiny.Utility;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Maps
{
    public sealed class MapFactory : KeyedCollection<int, Map>
    {
        private byte mChannel;

        public MapFactory(byte channel)
            : base()
        {
            mChannel = channel;
        }

        public void Load()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                using (DatabaseQuery query = Database.Query("SELECT * FROM map_data"))
                {
                    while (query.NextRow())
                    {
                        this.Add(new Map(mChannel, query));
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

            foreach (Map map in this)
            {
                map.SpawnPoints.Spawn();
            }
        }

        protected override int GetKeyForItem(Map item)
        {
            return item.MapleID;
        }
    }
}
