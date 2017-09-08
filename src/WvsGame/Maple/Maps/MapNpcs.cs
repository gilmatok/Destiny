using Destiny.Network;
using Destiny.Maple.Data;
using Destiny.Maple.Life;

namespace Destiny.Maple.Maps
{
    public sealed class MapNpcs : MapObjects<Npc>
    {
        public MapNpcs(Map map) : base(map) { }

        protected override void InsertItem(int index, Npc item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                using (Packet oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }

                item.AssignController();
            }
        }

        protected override void RemoveItem(int index)
        {
            if (DataProvider.IsInitialized)
            {
                Npc item = base.Items[index];

                item.Controller.ControlledNpcs.Remove(index);

                using (Packet oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            base.RemoveItem(index);
        }
    }
}
