using Destiny.Data;
using Destiny.IO;
using Destiny.Maple.Life;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedReactors : KeyedCollection<int, Reactor>
    {
        public CachedReactors()
            : base()
        {
            using (Log.Load("Reactors"))
            {
                foreach (Datum datum in new Datums("reactor_data").Populate())
                {
                    this.Add(new Reactor(datum));
                }

                foreach (Datum datum in new Datums("reactor_events").Populate())
                {
                    this[(int)datum["reactorid"]].States[(sbyte)datum["state"]] = new ReactorState(datum);
                }
            }
        }

        protected override int GetKeyForItem(Reactor item)
        {
            return item.MapleID;
        }
    }
}
