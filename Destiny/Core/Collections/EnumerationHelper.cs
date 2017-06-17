using System;
using System.Collections;
using System.Collections.Generic;

namespace Destiny.Core.Collections
{
    public abstract class EnumerationHelper<TKey, TValue> : IEnumerable<TValue>
    {
        public EnumerationHelper() { }

        public abstract TKey GetKeyForObject(TValue item);

        public TValue this[TKey key]
        {
            get
            {
                foreach (TValue item in this)
                {
                    if (EqualityComparer<TKey>.Default.Equals(key, this.GetKeyForObject(item)))
                    {
                        return item;
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        public bool Contains(TKey key)
        {
            foreach (TValue item in this)
            {
                if (EqualityComparer<TKey>.Default.Equals(key, this.GetKeyForObject(item)))
                {
                    return true;
                }
            }

            return false;
        }

        public abstract IEnumerator<TValue> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                int count = 0;

                foreach (TValue item in this)
                {
                    count++;
                }

                return count;
            }
        }
    }
}
