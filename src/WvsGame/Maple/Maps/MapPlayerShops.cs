using Destiny.Network;
using Destiny.Maple.Interaction;

namespace Destiny.Maple.Maps
{
    public sealed class MapPlayerShops : MapObjects<PlayerShop>
    {
        public MapPlayerShops(Map map) : base(map) { }

        protected override void InsertItem(int index, PlayerShop item)
        {
            lock (this)
            {
                base.InsertItem(index, item);

                using (Packet oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                PlayerShop item = base.Items[index];

                using (Packet oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket);
                }

                base.RemoveItem(index);
            }
        }
    }
}
