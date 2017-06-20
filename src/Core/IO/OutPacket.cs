using Destiny.Core.Network;
using Destiny.Maple;
using System;
using System.IO;
using System.Text;

namespace Destiny.Core.IO
{
    public sealed class OutPacket : PacketBase
    {
        private BinaryWriter m_writer;
        
        public OutPacket(short operationCode, int size = 64)
        {
            m_stream = new MemoryStream(size);
            m_writer = new BinaryWriter(m_stream, Encoding.ASCII);

            this.WriteShort(operationCode);
        }

        public OutPacket(SendOps operationCode) : this((short)operationCode) { }

        public OutPacket WriteBytes(byte[] value)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteByte(byte value = 0)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteSByte(sbyte value = 0)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteBool(bool value = false)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteShort(short value = 0)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteInt(int value = 0)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteLong(long value = 0)
        {
            m_writer.Write(value);

            return this;
        }

        public OutPacket WriteString(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                m_writer.Write(value[i]);
            }

            return this;
        }

        public OutPacket WritePaddedString(string value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                {
                    m_writer.Write(value[i]);
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
            m_writer.Write(value.ToFileTimeUtc());

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
            m_writer.Dispose();
        }
    }
}
