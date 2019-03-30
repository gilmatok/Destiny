using System;
using System.Linq;
using Destiny.IO;
using Destiny.Maple.Characters;
using Destiny.Network;
using Destiny.Threading;

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
                item.Expiry.Dispose();
            }

            item.Expiry = new Delay(() =>
            {
                if (item.Map == this.Map)
                {
                    this.Remove(item);
                }
            }, Drop.ExpiryTime);

            lock (this.Map.Characters)
            {
                foreach (Character character in this.Map.Characters)
                {
                    using (Packet oPacket = item.GetCreatePacket(item.Owner == null ? character : null))
                    {
                        character.Client.Send(oPacket);
                    }
                }
            }
        }

        protected override void RemoveItem(int index)
        {
			try
			{
				Drop item = base.Items[index];

				if (item.Expiry != null)
				{
					item.Expiry.Dispose();
				}

				using (Packet oPacket = item.GetDestroyPacket())
				{
					this.Map.Broadcast(oPacket);
				}
				
				base.RemoveItem(index);
			}
			catch (ArgumentOutOfRangeException)
			{
				Log.SkipLine();
				Tracer.TraceErrorMessage("Failed to remove item!\nIndex outside of range.");
				Log.SkipLine();
			}
			catch (Exception e)
			{
				Log.SkipLine();
				Log.Error("MapDrops-RemoveItem: exception occurred: {0}", e);
				Log.SkipLine();
			}
		}
    }
}