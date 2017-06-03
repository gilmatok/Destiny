using Destiny.Game.Data;
using Destiny.Game.Maps;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class MapFactory : Dictionary<int, Map>
    {
        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public new Map this[int mapleID]
        {
            get
            {
                if (!base.ContainsKey(mapleID))
                {
                    Map map = new Map(mapleID, this.World, this.Channel);

                    foreach (MapMobSpawnData mob in map.Data.Mobs)
                    {
                        map.Mobs.Add(new Mob(mob));
                    }

                    foreach (MapNpcSpawnData npc in map.Data.Npcs)
                    {
                        map.Npcs.Add(new Npc(npc));
                    }

                    foreach (MapPortalData portal in map.Data.Portals)
                    {
                        map.Portals.Add(new Portal(portal));
                    }

                    base.Add(mapleID, map);
                }

                return base[mapleID];
            }
        }

        public MapFactory(byte world, byte channel)
            : base()
        {
            this.World = world;
            this.Channel = channel;
        }
    }
}
