using Destiny.Core.Network;
using Destiny.Maple;
using System;
using System.IO;

namespace Destiny.Core.Network
{
    public sealed class OutPacket : PacketBase
    {
        private const int DefaultBufferSize = 32;

        private BinaryWriter mWriter;

        public OutPacket()
        {
            mStream = new MemoryStream(DefaultBufferSize);
            mWriter = new BinaryWriter(mStream);
        }

        public OutPacket(short operationCode, int size = DefaultBufferSize)
        {
            mStream = new MemoryStream(size);
            mWriter = new BinaryWriter(mStream);

            this.WriteShort(operationCode);
        }

        public OutPacket(ServerOperationCode operationCode) : this((short)operationCode) { }

        public OutPacket WriteBytes(params byte[] value)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteByte(byte value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteSByte(sbyte value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteBool(bool value = false)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteShort(short value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteUShort(ushort value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteInt(int value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteUInt(uint value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteLong(long value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        private OutPacket WriteString(string value)
        {
            value = value ?? string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                mWriter.Write(value[i]);
            }

            return this;
        }

        public OutPacket WritePaddedString(string value, int length)
        {
            value = value ?? string.Empty;

            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                {
                    mWriter.Write(value[i]);
                }
                else
                {
                    WriteByte();
                }
            }

            return this;
        }

        public OutPacket WriteMapleString(string fmt, params object[] args)
        {
            fmt = fmt ?? string.Empty;

            string final = string.Format(fmt, args);

            this.WriteShort((short)final.Length);
            this.WriteString(final);

            return this;
        }

        public OutPacket WriteZero(int count)
        {
            for (int i = 0; i < count; i++)
                WriteByte();

            return this;
        }

        public OutPacket WriteDateTime(DateTime value)
        {
            mWriter.Write(value.ToFileTimeUtc());

            return this;
        }

        public OutPacket WritePoint(Point value)
        {
            WriteShort(value.X);
            WriteShort(value.Y);

            return this;
        }

        protected override void CustomDispose()
        {
            mWriter.Dispose();
        }
    }
}
