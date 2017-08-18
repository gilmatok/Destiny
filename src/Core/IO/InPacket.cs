using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Packets;
using System;

namespace Destiny.Core.IO
{
    public sealed class InPacket
    {
        private int mIndex;
        private readonly byte[] mBuffer;

        public static LogLevel LogLevel { get; set; }

        public ClientOperationCode OperationCode { get; private set; }

        public int Remaining
        {
            get
            {
                return this.mBuffer.Length - this.mIndex;
            }
        }

        public InPacket(byte[] buffer)
        {
            mIndex = 0;
            mBuffer = buffer;

            this.OperationCode = (ClientOperationCode)this.ReadShort();
        }

        private void CheckLength(int count)
        {
            if (mIndex + count > mBuffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void Skip(int count)
        {
            this.CheckLength(count);

            mIndex += count;
        }

        public bool ReadBool()
        {
            return this.ReadByte() == 1;
        }

        public byte ReadByte()
        {
            return mBuffer[mIndex++];
        }

        public byte[] ReadBytes(int count)
        {
            this.CheckLength(count);

            byte[] value = new byte[count];

            Buffer.BlockCopy(mBuffer, mIndex, value, 0, count);

            mIndex += count;

            return value;
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

        public unsafe ushort ReadUShort()
        {
            CheckLength(2);

            ushort value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(ushort*)(ptr + mIndex);
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

        public unsafe uint ReadUInt()
        {
            CheckLength(4);

            uint value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(uint*)(ptr + mIndex);
            }

            mIndex += 4;

            return value;
        }

        public unsafe long ReadLong()
        {
            this.CheckLength(8);

            long value;

            fixed (byte* ptr = mBuffer)
            {
                value = *(long*)(ptr + mIndex);
            }

            mIndex += 8;

            return value;
        }

        public string ReadString(int count)
        {
            this.CheckLength(count);

            char[] value = new char[count];

            for (int i = 0; i < count; i++)
            {
                value[i] = (char)this.ReadByte();
            }

            return new string(value);
        }

        public string ReadMapleString()
        {
            short length = ReadShort();

            return ReadString(length);
        }

        public Point ReadPoint()
        {
            return new Point(this.ReadShort(), this.ReadShort());
        }

        public byte[] ToArray()
        {
            byte[] value = new byte[mBuffer.Length];

            Buffer.BlockCopy(mBuffer, 0, value, 0, mBuffer.Length);

            return value;
        }
    }
}
