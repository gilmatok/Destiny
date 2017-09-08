using System;
using System.Collections.Generic;
using System.Threading;

namespace Destiny.Collections
{
    public class PendingKeyedQueue<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable
    {
        private ManualResetEvent QueueDone = new ManualResetEvent(false);

        public PendingKeyedQueue() : base() { }

        public void Enqueue(TKey key, TValue value)
        {
            base.Add(key, value);

            this.QueueDone.Set();
        }

        public TValue Dequeue(TKey key)
        {
            while (!this.ContainsKey(key))
            {
                this.QueueDone.WaitOne();
            }

            TValue value = this[key];

            this.Remove(key);

            this.QueueDone.Reset();

            return value;
        }

        public void Dispose()
        {
            this.QueueDone.Dispose();
        }
    }
}