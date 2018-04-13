using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.Network;

namespace Destiny.Maple.Maps
{
    public sealed class MapObjectMist : MapObjects<Mist>
    {
        public MapObjectMist(Map map) : base(map) { }

        protected override void InsertItem(int index, Mist item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                using (Packet oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }
        }

    }
}