using Destiny.Game;
using Destiny.Network;
using System;
using System.IO;
using System.Text;

namespace Destiny.Core.IO
{
    public sealed class OutPacket : PacketBase
    {
        private BinaryWriter mWriter;

        public OutPacket(short operationCode, int size = 64)
        {
            mStream = new MemoryStream(size);
            mWriter = new BinaryWriter(mStream, Encoding.ASCII);

            this.WriteShort(operationCode);
        }

        public OutPacket(SendOps operationCode) : this((short)operationCode) { }

        public OutPacket WriteBytes(byte[] value)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteByte(byte value = 0)
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

        public OutPacket WriteInt(int value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteLong(long value = 0)
        {
            mWriter.Write(value);

            return this;
        }

        public OutPacket WriteString(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                mWriter.Write(value[i]);
            }

            return this;
        }

        public OutPacket WritePaddedString(string value, int length)
        {
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
            string final = string.Format(fmt, args);

            WriteShort((short)final.Length);
            WriteString(final);

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
