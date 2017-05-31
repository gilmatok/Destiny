using Destiny.Game.Maps;
using Destiny.Network.Packet;
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

                    this.Parent.Client.Send(NpcPacket.NpcControlRequest(item));
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

                if (item.Controller.Client.IsAlive)
                {
                    this.Parent.Client.Send(NpcPacket.NpcControlCancel(item.ObjectID));
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
