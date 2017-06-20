using Destiny.Maple;
using System;
using System.IO;
using System.Text;

namespace Destiny.Core.IO
{
    public sealed class InPacket : PacketBase
    {
        private BinaryReader m_reader;

        public short OperationCode { get; private set; }

        public InPacket(byte[] buffer)
        {
            m_stream = new MemoryStream(buffer, false);
            m_reader = new BinaryReader(m_stream, Encoding.ASCII);

            this.OperationCode = this.ReadShort();
        }

        public byte[] ReadBytes(int count)
        {
            return m_reader.ReadBytes(count);
        }

        public byte ReadByte()
        {
            return m_reader.ReadByte();
        }

        public bool ReadBool()
        {
            return m_reader.ReadBoolean();
        }

        public short ReadShort()
        {
            return m_reader.ReadInt16();
        }

        public int ReadInt()
        {
            return m_reader.ReadInt32();
        }

        public long ReadLong()
        {
            return m_reader.ReadInt64();
        }

        public string ReadString(int length)
        {
            return new string(m_reader.ReadChars(length));
        }

        public string ReadMapleString()
        {
            return ReadString(ReadShort());
        }

        public DateTime ReadDateTime()
        {
            return DateTime.FromFileTimeUtc(this.ReadLong());
        }

        public Point ReadPoint()
        {
            return new Point(this.ReadShort(), this.ReadShort());
        }

        protected override void CustomDispose()
        {
            m_reader.Dispose();
        }
    }
}
