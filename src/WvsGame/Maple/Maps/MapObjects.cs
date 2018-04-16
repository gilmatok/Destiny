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

            try
            {
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
            if (index >= 0 && index < int.MaxValue)
            {
                if (base.Items.Count < index)
                {
                    Log.SkipLine();
                    Tracer.TraceErrorMessage("Failed to remove item, there is less items in base then index points to!");
                    Log.SkipLine();
                    //Log.Inform("ERROR: MapObjects-RemoveItem() failed to remove item! Index: {0} \n Theres less items then index points to: {1}", index, base.Items.Count);
                }

                else if (base.Items.Count >= index)
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
                    catch (Exception e)
                    {
                        Log.SkipLine();
                        Tracer.TraceErrorMessage("Failed to remove item! \n Exception occurred.");
                        Log.SkipLine();
                        //Log.Inform("ERROR: MapObjects-RemoveItem() failed to remove item! ItemIndex: {0} \n Exception occurred: {1}", index, e);
                    }
                }

                else
                {
                    Log.SkipLine();
                    Tracer.TraceErrorMessage("Failed to remove item!.");
                    Log.SkipLine();
                    //Log.Inform("ERROR: MapObjects-RemoveItem() failed to remove item! ItemIndex: {0}, Items.Count: {1}", index, Items.Count);
                }             
            }

            else
            {
                Log.SkipLine();
                Tracer.TraceErrorMessage("Failed item index out of bounds!!.");
                Log.SkipLine();
                //Log.Error("ERROR: MapObjects-RemoveItem() index out of bounds! ItemIndex: {0}", index);
            }
        }
    }
}
