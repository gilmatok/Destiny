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
			try
			{
				item.Map = this.Map;

				if (!(item is Character)  && !(item is Portal))
				{
					item.ObjectID = this.Map.AssignObjectID();
				}
				
                base.InsertItem(index, item);
            }
            catch(Exception e)
            {
                Log.SkipLine();
                Tracer.TraceErrorMessage("Failed to insert item!");
                Log.SkipLine();
                //Log.Inform("ERROR: MapObjects-InsertItem() failed to insert item! Index: {0} \n Exception occurred: {1}", index, e);
            }     
        }

        protected override void RemoveItem(int index)
        {
			try
			{
                T item = base.Items[index];
                item.Map = null;

                if (!(item is Character) && !(item is Portal))
                {
                    item.ObjectID = -1;
                }

                base.RemoveItem(index);  
            }
			catch (ArgumentOutOfRangeException)
            {
                Log.SkipLine();
                Tracer.TraceErrorMessage("Failed to remove item! \n Index outside of range.");
                Log.SkipLine();
            }
        }
    }
}
