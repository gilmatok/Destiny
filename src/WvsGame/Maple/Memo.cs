using Destiny.Data;
using System;
using Destiny.IO;

namespace Destiny.Maple
{
    public sealed class Memo
    {
        public int ID { get; set; }
        public string Sender { get; private set; }
        public string Message { get; private set; }
        public DateTime Received { get; private set; }

        public Memo(Datum datum)
        {
            this.ID = (int)datum["ID"];
            this.Sender = (string)datum["Sender"];
            this.Message = (string)datum["Message"];
            this.Received = (DateTime)datum["Received"];
        }

        public void Delete()
        {
            Database.Delete("memos", "ID = {0}", this.ID);
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteString(this.Sender + " ") // NOTE: Space is intentional.
                    .WriteString(this.Message)
                    .WriteDateTime(this.Received)
                    .WriteByte(3); // TODO: Memo kind (0 - None, 1 - Fame, 2 - Gift).

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }
    }
}
