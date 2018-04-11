using System;
using System.IO;
using System.Net;

namespace Destiny.IO
{
    public class ByteBuffer : IDisposable
    {
        private int position;
        private MemoryStream Stream { get; set; }
        private BinaryWriter Writer { get; set; }
        private BinaryReader Reader { get; set; }

        public byte[] Array { get; private set; }
        public int Offset { get; private set; }
        public int Capacity { get; private set; }
        public int Limit { get; set; }
        public bool HasFlipped { get; private set; }

        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                this.position = value;
                this.Stream.Position = this.Position + this.Offset;
            }
        }

        public int Remaining
        {
            get
            {
                return this.Limit - this.Position;
            }
        }

        public ByteBuffer(int capacity = Application.DefaultBufferSize)
        {
            this.Capacity = capacity;
            this.Array = new byte[this.Capacity];

            this.Stream = new MemoryStream(this.Array);
            this.Writer = new BinaryWriter(this.Stream);
            this.Reader = new BinaryReader(this.Stream);

            this.Limit = this.Capacity;
            this.Offset = 0;
            this.Position = 0;
        }

        public ByteBuffer(byte[] data)
        {
            this.Capacity = data.Length;
            this.Array = data;

            this.Stream = new MemoryStream(this.Array);
            this.Writer = new BinaryWriter(this.Stream);
            this.Reader = new BinaryReader(this.Stream);

            this.Limit = this.Capacity;
            this.Offset = 0;
            this.Position = 0;
        }

        private ByteBuffer(byte[] array, int offset, int capacity)
        {
            this.Array = array;

            this.Stream = new MemoryStream(this.Array);
            this.Writer = new BinaryWriter(this.Stream);
            this.Reader = new BinaryReader(this.Stream);

            this.Offset = offset;
            this.Capacity = capacity;
            this.Limit = this.Capacity;
            this.Position = 0;
        }

        public byte this[int index]
        {
            get
            {
                return this.Array[index];
            }
            set
            {
                this.Array[index] = value;
            }
        }

        public byte[] GetContent()
        {
            byte[] ba = new byte[this.Remaining];
            Buffer.BlockCopy(this.Array, this.Position + this.Offset, ba, 0, this.Remaining);
            return ba;
        }

        public ByteBuffer Skip(int count)
        {
            this.Position += count;
            
            return this;
        }

        public void Flip()
        {
            this.Limit = this.Position;
            this.Position = 0;
            this.HasFlipped = true;
        }

        public void SafeFlip()
        {
            if (!this.HasFlipped)
            {
                this.Flip();
            }
        }

        public ByteBuffer Slice()
        {
            return new ByteBuffer(this.Array, this.Position, this.Remaining);
        }

        public void Dispose()
        {
            this.Reader.Dispose();
            this.Writer.Dispose();
            this.Stream.Dispose();
        }

        public ByteBuffer WriteBytes(params byte[] collection)
        {
            this.Writer.Write(collection);
            this.Position += collection.Length;

            return this;
        }

        public ByteBuffer WriteByte(byte item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(byte);

            return this;
        }

        public ByteBuffer WriteSByte(sbyte item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(sbyte);

            return this;
        }

        public ByteBuffer WriteShort(short item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(short);

            return this;
        }

        public ByteBuffer WriteUShort(ushort item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(ushort);

            return this;
        }

        public ByteBuffer WriteInt(int item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(int);

            return this;
        }

        public ByteBuffer WriteUInt(uint item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(uint);

            return this;
        }

        public ByteBuffer WriteLong(long item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(long);

            return this;
        }

        public ByteBuffer WriteFloat(float item = 0)
        {
            this.Writer.Write(item);
            this.Position += sizeof(float);

            return this;
        }

        public ByteBuffer WriteBool(bool item)
        {
            this.Writer.Write(item);
            this.Position += sizeof(bool);

            return this;
        }

        public ByteBuffer WriteString(string item, params object[] args)
        {
            item = item ?? string.Empty;

            if (item != null)
            {
                item = string.Format(item, args);
            }

            this.Writer.Write((short)item.Length);

            foreach (char c in item)
            {
                this.Writer.Write(c);
            }

            this.Position += item.Length + sizeof(short);

            return this;
        }

        public ByteBuffer WriteStringFixed(string item, int length)
        {
            foreach (char c in item)
            {
                this.Writer.Write(c);
            }

            for (int i = item.Length; i < length; i++)
            {
                this.Writer.Write((byte)0);
            }

            this.Position += length;

            return this;
        }

        public ByteBuffer WriteDateTime(DateTime item)
        {
            this.Writer.Write((long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            this.Position += sizeof(long);

            return this;
        }

        public ByteBuffer WriteKoreanDateTime(DateTime item)
        {
            this.Writer.Write((long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds * 10000 + 116444592000000000L);
            this.Position += sizeof(long);

            return this;
        }
        
        public ByteBuffer WriteIPAddress(IPAddress value)
        {
            this.Writer.Write(value.GetAddressBytes());
            this.Position += 4;

            return this;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = this.Reader.ReadBytes(count);
            this.Position += count;
            return result;
        }

        public byte[] ReadBytes()
        {
            return this.ReadBytes(this.Remaining);
        }

        public byte ReadByte()
        {
            byte result = this.Reader.ReadByte();
            this.Position += sizeof(byte);
            return result;
        }

        public sbyte ReadSByte()
        {
            sbyte result = this.Reader.ReadSByte();
            this.Position += sizeof(sbyte);
            return result;
        }

        public short ReadShort()
        {
            short result = this.Reader.ReadInt16();
            this.Position += sizeof(short);
            return result;
        }

        public ushort ReadUShort()
        {
            ushort result = this.Reader.ReadUInt16();
            this.Position += sizeof(ushort);
            return result;
        }

        public int ReadInt()
        {
            var count = this.Reader.BaseStream.Length / sizeof(int);

            /*for (var i = 0; i < count; i++)
            {
                int v = this.Reader.ReadInt32();
            }*/

#if DEBUG
            Log.Inform("ByteBuffer-ReadInt() count of int sized lenghts in reader stream: {0}", count);
#endif

            int result = this.Reader.ReadInt32();
            this.Position += sizeof(int);
            return result;
        }

        public uint ReadUInt()
        {
            uint result = this.Reader.ReadUInt32();
            this.Position += sizeof(uint);
            return result;
        }

        public long ReadLong()
        {
            long result = this.Reader.ReadInt64();
            this.Position += sizeof(long);
            return result;
        }

        public float ReadFloat()
        {
            float result = this.Reader.ReadSingle();
            this.Position += sizeof(float);
            return result;
        }

        public bool ReadBool()
        {
            bool result = this.Reader.ReadBoolean();
            this.Position += sizeof(bool);
            return result;
        }

        public string ReadString()
        {
            short count = this.Reader.ReadInt16();

            char[] result = new char[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = (char)this.Reader.ReadByte();
            }

            this.Position += count + sizeof(short);

            return new string(result);
        }

        public IPAddress ReadIPAddress()
        {
            IPAddress result = new IPAddress(this.Reader.ReadBytes(4));
            this.Position += 4;
            return result;
        }
    }
}
