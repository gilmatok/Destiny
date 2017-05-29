using Destiny.Data;
using Destiny.Game;
using Destiny.Utility;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class MapFactory
    {
        private const int MAP_UNLOAD_TIME = 3600000;

        public byte World { get; private set; }
        public byte Channel { get; private set; }

        private Dictionary<int, Map> mActiveMaps;

        public MapFactory(byte world, byte channel)
        {
            this.World = world;
            this.Channel = channel;

            mActiveMaps = new Dictionary<int, Map>();
        }

        public Map this[int mapleID]
        {
            get
            {
                Map map = mActiveMaps.GetOrDefault(mapleID, null);

                if (map == null)
                {
                    map = new Map(mapleID);

                    MapData data = MasterServer.Instance.Data.Maps[mapleID];

                    data.NPCs.ForEach(n => map.Npcs.Add(new Npc(n)));

                    mActiveMaps.Add(mapleID, map);
                }

                return map;
            }
        }
    }
}
