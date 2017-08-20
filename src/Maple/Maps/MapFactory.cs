using Destiny.Maple.Life;
using Destiny.Server;
using System.Collections.ObjectModel;

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
                    Map map = new Map(this, key);

                    foreach (Npc npc in map.CachedReference.Npcs)
                    {
                        map.Npcs.Add(npc);
                    }

                    foreach (Reactor reactor in map.CachedReference.Reactors)
                    {
                        map.Reactors.Add(reactor);
                    }

                    foreach (Foothold foothold in map.CachedReference.Footholds)
                    {
                        map.Footholds.Add(foothold);
                    }

                    foreach (Seat seat in map.CachedReference.Seats)
                    {
                        map.Seats.Add(seat);
                    }

                    foreach (Portal portal in map.CachedReference.Portals)
                    {
                        map.Portals.Add(portal);
                    }

                    foreach (SpawnPoint spawnPoint in map.CachedReference.SpawnPoints)
                    {
                        map.SpawnPoints.Add(spawnPoint);
                    }

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
