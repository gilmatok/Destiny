using System;
using System.IO;

namespace Destiny.Core.IO
{
    public abstract class PacketBase : IDisposable
    {
        protected MemoryStream mStream;

        public int Length
        {
            get
            {
                return (int)mStream.Length;
            }
        }

        public int Position
        {
            get
            {
                return (int)mStream.Position;
            }
            set
            {
                mStream.Position = value;
            }
        }

        public int Remaining
        {
            get
            {
                return this.Length - this.Position;
            }
        }

        public void Skip(int count)
        {
            this.Position += count;
        }

        public byte[] ToArray()
        {
            return mStream.ToArray();
        }

        public override string ToString()
        {
            string ret = "";

            foreach (byte b in this.ToArray())
            {
                ret += string.Format("{0:X2} ", b);
            }

            return ret;
        }

        protected virtual void CustomDispose() { }

        public void Dispose()
        {
            this.CustomDispose();

            mStream.Dispose();
        }
    }
}
