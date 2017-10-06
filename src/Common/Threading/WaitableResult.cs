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
            mEvent.Set();

            mValue = value;
        }

        public T Value { get { return mValue; } }
    }
}
