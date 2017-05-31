using Destiny.Game;
using System;
using System.IO;
using System.Text;

namespace Destiny.Core.IO
{
    public sealed class InPacket : PacketBase
    {
        private BinaryReader mReader;

        public short OperationCode { get; private set; }

        public InPacket(byte[] buffer)
        {
            mStream = new MemoryStream(buffer, false);
            mReader = new BinaryReader(mStream, Encoding.ASCII);

            this.OperationCode = this.ReadShort();
        }

        public byte[] ReadBytes(int count)
        {
            return mReader.ReadBytes(count);
        }

        public byte ReadByte()
        {
            return mReader.ReadByte();
        }

        public bool ReadBool()
        {
            return mReader.ReadBoolean();
        }

        public short ReadShort()
        {
            return mReader.ReadInt16();
        }

        public int ReadInt()
        {
            return mReader.ReadInt32();
        }

        public long ReadLong()
        {
            return mReader.ReadInt64();
        }

        public string ReadString(int length)
        {
            return new string(mReader.ReadChars(length));
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
            mReader.Dispose();
        }
    }
}
