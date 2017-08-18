using Destiny.Maple;
using Destiny.Packets;
using System;

namespace Destiny.Core.IO
{
    public sealed class OutPacket : IDisposable
    {
        private int mIndex;
        private byte[] mBuffer;

        public ServerOperationCode OperationCode { get; private set; }

        public OutPacket(ServerOperationCode operationCode)
        {
            mIndex = 0;
            mBuffer = new byte[1024];

            this.OperationCode = operationCode;

            this.WriteUShort((ushort)this.OperationCode);
        }

        private void EnsureLength(int count)
        {
            if (mIndex + count > mBuffer.Length)
            {
                Array.Resize<byte>(ref mBuffer, mBuffer.Length * 2);
            }
        }

        public OutPacket Skip(int count)
        {
            this.EnsureLength(count);

            mIndex += count;

            return this;
        }

        public OutPacket WriteBool(bool value = false) => this.WriteByte((byte)(value ? 1 : 0));

        public unsafe OutPacket WriteByte(byte value = 0)
        {
            int size = sizeof(byte);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteSByte(sbyte value = 0)
        {
            int size = sizeof(sbyte);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(sbyte*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        // TODO: Better logic.
        public OutPacket WriteBytes(params byte[] value)
        {
            foreach (byte b in value)
            {
                this.WriteByte(b);
            }

            return this;
        }

        public unsafe OutPacket WriteShort(short value = 0)
        {
            int size = sizeof(short);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(short*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteUShort(ushort value = 0)
        {
            int size = sizeof(ushort);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(ushort*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteInt(int value = 0)
        {
            int size = sizeof(int);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(int*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteUInt(uint value = 0)
        {
            int size = sizeof(uint);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(uint*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteLong(long value = 0)
        {
            int size = sizeof(long);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(long*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public unsafe OutPacket WriteULong(ulong value = 0)
        {
            int size = sizeof(ulong);

            this.EnsureLength(size);

            fixed (byte* ptr = mBuffer)
            {
                *(ulong*)(ptr + mIndex) = value;
            }

            mIndex += size;

            return this;
        }

        public OutPacket WriteString(string value)
        {
            this.WriteShort((short)value.Length);

            foreach (char c in value)
            {
                this.WriteByte((byte)c);
            }

            return this;
        }

        public OutPacket WritePaddedString(string value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                {
                    this.WriteByte((byte)value[i]);
                }
                else
                {
                    this.WriteByte();
                }
            }

            return this;
        }

        public OutPacket WritePoint(Point value)
        {
            this.WriteShort(value.X);
            this.WriteShort(value.Y);

            return this;
        }

        public OutPacket WriteDateTime(DateTime value)
        {
            this.WriteLong(value.ToFileTimeUtc());

            return this;
        }

        public byte[] ToArray()
        {
            return mBuffer; // TODO: Better logic.
        }

        public void Dispose()
        {
            // TODO: Dispose logic.
        }
    }
}
