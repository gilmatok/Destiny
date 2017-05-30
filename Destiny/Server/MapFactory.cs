using Destiny.Game.Maps;
using System.Collections.Generic;

namespace Destiny.Server
{
    public sealed class MapFactory : SortedDictionary<int, Map>
    {
        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public new Map this[int mapleID]
        {
            get
            {
                if (!base.ContainsKey(mapleID))
                {
                    Map map = new Map(mapleID);

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
