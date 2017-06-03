using Destiny.Core.IO;
using Destiny.Game.Maps;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Game.Characters
{
    public class ControlledNpcs : KeyedCollection<int, Npc>
    {
        public Character Parent { get; private set; }

        public ControlledNpcs(Character parent)
        {
            this.Parent = parent;
        }

        protected override void InsertItem(int index, Npc item)
        {
            lock (this)
            {
                if (this.Parent.Client.IsAlive)
                {
                    item.Controller = this.Parent;

                    base.InsertItem(index, item);

                    using (OutPacket oPacket = item.GetControlRequestPacket())
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
                    using (OutPacket oPacket = item.GetControlCancelPacket())
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
