using Destiny.Core.Data;
using System;
using Destiny.Core.Network;

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
            using (OutPacket oPacket = new OutPacket())
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteMapleString(this.Sender + " ") // NOTE: Space is intentional.
                    .WriteMapleString(this.Message)
                    .WriteDateTime(this.Received)
                    .WriteByte(3); // TODO: Memo kind (0 - None, 1 - Fame, 2 - Gift).

                return oPacket.ToArray();
            }
        }
    }
}
