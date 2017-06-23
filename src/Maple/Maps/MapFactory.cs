using System.Collections.ObjectModel;

namespace Destiny.Maple.Maps
{
    public sealed class MapFactory : KeyedCollection<int, Map>
    {
        private byte mChannel;

        // NOTE: To ensure external map factories are functioning properly,
        // the default channel is 0 so the server can obtain data about maps
        // regardless of not being owned by a channel.
        public MapFactory(byte channel = 0)
            : base()
        {
            mChannel = channel;
        }

        public new Map this[int key]
        {
            get
            {
                if (!base.Contains(key))
                {
                    this.Add(new Map(key, mChannel));
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
