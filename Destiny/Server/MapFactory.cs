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

        public Map this[int identifier]
        {
            get
            {
                Map map = mActiveMaps.GetOrDefault(identifier, null);

                if (map == null)
                {
                    map = new Map(identifier);

                    mActiveMaps.Add(identifier, map);
                }

                return map;
            }
        }
    }
}
