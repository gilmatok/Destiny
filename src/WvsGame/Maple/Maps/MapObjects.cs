using System;
using Destiny.Maple.Characters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Destiny.IO;

namespace Destiny.Maple.Maps
{
    public abstract class MapObjects<T> : KeyedCollection<int, T> where T : MapObject
    {
        public Map Map { get; private set; }

        public MapObjects(Map map)
        {
            this.Map = map;
        }

        public IEnumerable<T> GetInRange(MapObject reference, int range)
        {
            foreach (T loopObject in this)
            {
                if (reference.Position.DistanceFrom(loopObject.Position) <= range)
                {
                    yield return loopObject;
                }
            }
        }

        protected override int GetKeyForItem(T item)
        {
            return item.ObjectID;
        }

        protected override void InsertItem(int index, T item)
        {
            item.Map = this.Map;

            if (!(item is Character)  && !(item is Portal))
            {
                item.ObjectID = this.Map.AssignObjectID();
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index >= 0 && index < int.MaxValue)
            {
                T item = base.Items[index];
                item.Map = null;

                if (!(item is Character) && !(item is Portal))
                {
                    item.ObjectID = -1;
                }

                try
                {
                    base.RemoveItem(index);
                }
                catch(Exception e)
                {
                    Log.SkipLine();
                    Log.Inform("ERROR: MapleObjects-RemoveItem() failed to remove item! Index: {0} \n Exception occured: {1}", index, e);
                    Log.SkipLine();
                }
            }
            else
            {
                Log.SkipLine();
                Log.Error("ERROR: MapleObjects-RemoveItem() index out of bounds! Index: {0}", index);
                Log.SkipLine();
            }
        }
    }
}
