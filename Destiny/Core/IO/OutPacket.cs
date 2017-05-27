using Destiny.Network;
using System;
using System.IO;

namespace Destiny.Core.IO
{
    public sealed class OutPacket : IDisposable
    {
        public const int DefaultBufferSize = 32;

        private MemoryStream mStream;

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

        public OutPacket()
        {
            mStream = new MemoryStream(OutPacket.DefaultBufferSize);
        }

        public OutPacket(short operationCode, int size = OutPacket.DefaultBufferSize)
        {
            mStream = new MemoryStream(size);

            this.WriteShort(operationCode);
        }

        public OutPacket(SendOpcode operationCode) : this((short)operationCode) { }

        private void Append(long value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                mStream.WriteByte((byte)value);

                value >>= 8;
            }
        }

        public OutPacket WriteBool(bool value = false)
        {
            this.WriteByte(value ? (byte)1 : (byte)0);

            return this;
        }

        public OutPacket WriteByte(byte value = 0)
        {
            mStream.WriteByte(value);

            return this;
        }

        public OutPacket WriteBytes(params byte[] value)
        {
            foreach (byte b in value)
            {
                this.WriteByte(b);
            }

            return this;
        }

        public OutPacket WriteShort(short value = 0)
        {
            this.Append(value, 2);

            return this;
        }

        public OutPacket WriteInt(int value = 0)
        {
            this.Append(value, 4);

            return this;
        }

        public OutPacket WriteLong(long value = 0)
        {
            this.Append(value, 8);

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

        public OutPacket WriteStringFixed(string value, int length)
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

        public OutPacket WriteDateTime(DateTime value)
        {
            return this.WriteLong(value.ToFileTimeUtc());
        }

        public OutPacket WriteZero(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.WriteByte();
            }

            return this;
        }

        public byte[] ToArray()
        {
            return mStream.ToArray();
        }

        public void Dispose()
        {
            mStream.Dispose();

            mStream = null;
        }
    }
}
