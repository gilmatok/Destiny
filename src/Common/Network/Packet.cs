using Destiny.IO;

namespace Destiny.Network
{
    public class Packet : ByteBuffer
    {
        public static LogLevel LogLevel { get; set; }

        public short OperationCode { get; private set; }

        public Packet(byte[] data)
            : base(data)
        {
            this.OperationCode = this.ReadShort();
        }

        public Packet(short operationCode) 
            : base()
        {
            this.OperationCode = operationCode;

            this.WriteShort(this.OperationCode);
        }

        public Packet(ServerOperationCode operationCode) : this((short)operationCode) { }
        public Packet(InteroperabilityOperationCode operationCode) : this((short)operationCode) { }
    }
}
