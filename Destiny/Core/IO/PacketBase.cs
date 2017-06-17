using System;
using System.IO;

namespace Destiny.Core.IO
{
    public abstract class PacketBase : IDisposable
    {
        protected MemoryStream m_stream;

        public int Length
        {
            get
            {
                return (int)m_stream.Length;
            }
        }

        public int Position
        {
            get
            {
                return (int)m_stream.Position;
            }
            set
            {
                m_stream.Position = value;
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
            return m_stream.ToArray();
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

            m_stream.Dispose();
        }
    }
}
