using Destiny.Core.IO;
using Destiny.Core.Threading;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps
{
    public sealed class MapDrops : MapObjects<Drop>
    {
        public MapDrops(Map map) : base(map) { }

        protected override void InsertItem(int index, Drop item)
        {
            item.Picker = null;

            base.InsertItem(index, item);

            if (item.Expiry != null)
            {
                item.Expiry.Cancel();
            }

            item.Expiry = new Delay(Drop.ExpiryTime, () =>
            {
                if (item.Map == this.Map)
                {
                    this.Remove(item);
                }
            });

            item.Expiry.Execute();

            lock (this.Map.Characters)
            {
                foreach (Character character in this.Map.Characters)
                {
                    using (OutPacket oPacket = item.GetCreatePacket(item.Owner == null ? character : null))
                    {
                        character.Client.Send(oPacket);
                    }
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            Drop item = base.Items[index];

            if (item.Expiry != null)
            {
                item.Expiry.Cancel();
            }

            using (OutPacket oPacket = item.GetDestroyPacket())
            {
                this.Map.Broadcast(oPacket);
            }

            base.RemoveItem(index);
        }
    }
}
