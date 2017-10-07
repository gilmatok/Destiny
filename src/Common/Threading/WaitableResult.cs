using System.Threading;

namespace Destiny.Threading
{
    public sealed class WaitableResult<T> where T : struct
    {
        private T mValue;

        private ManualResetEvent mEvent;

        public WaitableResult()
        {
            mEvent = new ManualResetEvent(false);
        }

        public void Wait()
        {
            mEvent.WaitOne();
        }

        public void Set(T value)
        {
            mValue = value;

            mEvent.Set();
        }

        public T Value { get { return mValue; } }
    }
}
