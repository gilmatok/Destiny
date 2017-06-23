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
