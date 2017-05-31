using Destiny.Game.Maps;
using Destiny.Network.Packet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Game.Characters
{
    public class ControlledMobs : KeyedCollection<int, Mob>
    {
        public Character Parent { get; private set; }

        public ControlledMobs(Character parent)
        {
            this.Parent = parent;
        }

        protected override void InsertItem(int index, Mob item)
        {
            lock (this)
            {
                if (this.Parent.Client.IsAlive)
                {
                    item.Controller = this.Parent;

                    base.InsertItem(index, item);

                    this.Parent.Client.Send(MobPacket.MobControlRequest(item));
                }
                else
                {
                    item.AssignController();
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Mob item = base.Items[index];

                if (item.Controller.Client.IsAlive)
                {
                    this.Parent.Client.Send(MobPacket.MobControlCancel(item.ObjectID));
                }

                item.Controller = null;

                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (this)
            {
                List<Mob> toRemove = new List<Mob>();

                foreach (Mob mob in this)
                {
                    toRemove.Add(mob);
                }

                foreach (Mob mob in toRemove)
                {
                    this.Remove(mob);
                }
            }
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.ObjectID;
        }
    }
}
