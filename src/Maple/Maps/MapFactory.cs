using Destiny.Data;
using Destiny.Maple.Life;
using Destiny.Server;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Destiny.Maple.Maps
{
    public sealed class MapFactory : KeyedCollection<int, Map>
    {
        public ChannelServer Channel { get; private set; }

        public MapFactory(ChannelServer channel)
            : base()
        {
            this.Channel = channel;
        }

        public new Map this[int key]
        {
            get
            {
                if (!base.Contains(key))
                {
                    using (Database.TemporarySchema("mcdb"))
                    {
                        foreach (Datum datum in new Datums("map_data").Populate("mapid = {0}", key))
                        {
                            this.Add(new Map(this, datum));
                        }

                        foreach (Datum datum in new Datums("map_footholds").Populate("mapid = {0}", key))
                        {
                            this[key].Footholds.Add(new Foothold(datum));
                        }

                        foreach (Datum datum in new Datums("map_seats").Populate("mapid = {0}", key))
                        {
                            this[key].Seats.Add(new Seat(datum));
                        }

                        foreach (Datum datum in new Datums("map_portals").Populate("mapid = {0}", key))
                        {
                            Type implementedType = Assembly.GetExecutingAssembly().GetType("Destiny.Maple.Maps.Portals." + (string)datum["script"]);

                            if (implementedType != null)
                            {
                                this[key].Portals.Add((Portal)Activator.CreateInstance(implementedType, datum));
                            }
                            else
                            {
                                this[key].Portals.Add(new Portal(datum));
                            }
                        }

                        foreach (Datum datum in new Datums("map_life").Populate("mapid = {0}", key))
                        {
                            switch ((string)datum["life_type"])
                            {
                                case "npc":
                                    {
                                        Type implementedType = Assembly.GetExecutingAssembly().GetType("Destiny.Maple.Life.Npcs.Npc" + (int)datum["lifeid"]);

                                        if (implementedType != null)
                                        {
                                            this[key].Npcs.Add((Npc)Activator.CreateInstance(implementedType, datum));
                                        }
                                        else
                                        {
                                            this[key].Npcs.Add(new Npc(datum));
                                        }
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
