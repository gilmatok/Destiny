using Destiny.Network;
using System;

namespace Destiny.Core.IO
{
    public sealed class InPacket
    {
        private int mIndex;
        private byte[] mBuffer;

        public int Position
        {
            get
            {
                return mIndex;
            }
        }

        public int Available
        {
            get
            {
                return mBuffer.Length - mIndex;
            }
        }

        public short OperationCode { get; private set; }

        public InPacket(byte[] packet)
        {
            mBuffer = packet;
            mIndex = 0;

            this.OperationCode = this.ReadShort();
        }

        private void CheckLength(int length)
        {
            if (mIndex + length > mBuffer.Length || length < 0)
                throw new IndexOutOfRangeException();
        }

        public bool ReadBool()
        {
            return mBuffer[mIndex++] != 0;
        }
        public byte ReadByte()
        {
            return mBuffer[mIndex++];
        }
        public byte[] ReadBytes(int count)
        {
            CheckLength(count);
            var temp = new byte[count];
            Buffer.BlockCopy(mBuffer, mIndex, temp, 0, count);
            mIndex += count;
            return temp;
        }
        public unsafe short ReadShort()
        {
            CheckLength(2);

            short value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(short*)(ptr + mIndex);
            }

            mIndex += 2;

            return value;
        }
        public unsafe int ReadInt()
        {
            CheckLength(4);

            int value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(int*)(ptr + mIndex);
            }

            mIndex += 4;

            return value;
        }
        public unsafe long ReadLong()
        {
            CheckLength(8);

            long value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(long*)(ptr + mIndex);
            }

            mIndex += 8;

            return value;
        }

        public string ReadString(int count = 0)
        {
            if (count == 0)
            {
                count = this.ReadShort();
            }

            CheckLength(count);

            char[] final = new char[count];

            for (int i = 0; i < count; i++)
            {
                final[i] = (char)ReadByte();
            }

            return new string(final);
        }

        public void Skip(int count)
        {
            CheckLength(count);
            mIndex += count;
        }

        public byte[] ToArray()
        {
            var final = new byte[mBuffer.Length];
            Buffer.BlockCopy(mBuffer, 0, final, 0, mBuffer.Length);
            return final;
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
    }
}
