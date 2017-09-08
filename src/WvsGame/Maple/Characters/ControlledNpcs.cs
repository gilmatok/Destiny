using Destiny.Network;
using Destiny.Maple.Life;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public class ControlledNpcs : KeyedCollection<int, Npc>
    {
        public Character Parent { get; private set; }

        public ControlledNpcs(Character parent)
        {
            this.Parent = parent;
        }

        public void Move(Packet iPacket)
        {
            int objectID = iPacket.ReadInt();

            Npc npc;

            try
            {
                npc = this[objectID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            npc.Move(iPacket);
        }

        protected override void InsertItem(int index, Npc item)
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
                Npc item = base.Items[index];

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
                List<Npc> toRemove = new List<Npc>();

                foreach (Npc npc in this)
                {
                    toRemove.Add(npc);
                }

                foreach (Npc npc in toRemove)
                {
                    this.Remove(npc);
                }
            }
        }

        protected override int GetKeyForItem(Npc item)
        {
            return item.ObjectID;
        }
    }
}
