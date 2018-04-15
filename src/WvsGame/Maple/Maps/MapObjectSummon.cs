using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.Network;

namespace Destiny.Maple.Maps
{
    public sealed class MapObjectSummon : MapObjects<Summon>
    {
        public MapObjectSummon(Map map) : base(map)
        { }

        protected override void InsertItem(int index, Summon item)
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
