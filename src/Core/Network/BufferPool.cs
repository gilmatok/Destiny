using System;
using System.Collections.Concurrent;

namespace Destiny.Core.Network
{
    public static class BufferPool
    {
        public const int BufferSize = 1024;

        private static ConcurrentBag<byte[]> sBufferPool = new ConcurrentBag<byte[]>();

        public static byte[] Get()
        {
            byte[] buffer;

            if (BufferPool.sBufferPool.TryTake(out buffer) == false)
            {
                buffer = new byte[BufferPool.BufferSize];
            }

            return buffer;
        }

        public static void Put(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            //if (buffer.Length != BufferPool.BufferSize)
            //{
            //    throw new ArgumentOutOfRangeException("buffer");
            //}

            Array.Resize<byte>(ref buffer, BufferSize);

            Array.Clear(buffer, 0, BufferPool.BufferSize);

            BufferPool.sBufferPool.Add(buffer);
        }
    }
}
