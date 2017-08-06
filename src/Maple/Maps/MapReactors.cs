using Destiny.Core.IO;
using Destiny.Maple.Data;
using Destiny.Maple.Life.Reactors;

namespace Destiny.Maple.Maps
{
    public sealed class MapReactors : MapObjects<Reactor>
    {
        public MapReactors(Map map) : base(map) { }

        protected override void InsertItem(int index, Reactor item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                using (OutPacket oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            if (DataProvider.IsInitialized)
            {
                Reactor item = base.Items[index];

                using (OutPacket oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            base.RemoveItem(index);
        }
    }
}
