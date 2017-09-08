using Destiny.Network;
using Destiny.Maple.Life;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public class ControlledMobs : KeyedCollection<int, Mob>
    {
        public Character Parent { get; private set; }

        public ControlledMobs(Character parent)
        {
            this.Parent = parent;
        }

        public void Move(Packet iPacket)
        {
            int objectID = iPacket.ReadInt();

            Mob mob;

            try
            {
                mob = this[objectID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            mob.Move(iPacket);
        }

        protected override void InsertItem(int index, Mob item)
        {
            lock (this)
            {
                if (this.Parent.Client.IsAlive)
                {
                    item.Controller = this.Parent;

                    base.InsertItem(index, item);

                    using (Packet oPacket = item.GetControlRequestPacket())
                    {
                        this.Parent.Client.Send(oPacket);
                    }
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

                if (this.Parent.Client.IsAlive)
                {
                    using (Packet oPacket = item.GetControlCancelPacket())
                    {
                        this.Parent.Client.Send(oPacket);
                    }
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
