using Destiny.Core.IO;
using Destiny.Core.Data;
using System;

namespace Destiny.Maple
{
    public sealed class Memo
    {
        public int ID { get; set; }
        public string Sender { get; private set; }
        public string Message { get; private set; }
        public DateTime Received { get; private set; }
        public bool IsDeleted { get; private set; }

        public Memo(Datum datum)
        {
            this.ID = (int)datum["ID"];
            this.Sender = (string)datum["Sender"];
            this.Message = (string)datum["Message"];
            this.Received = (DateTime)datum["Received"];
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.ID)
                .WriteString(this.Sender + " ") // NOTE: Nexon forgot a space.
                .WriteString(this.Message)
                .WriteDateTime(this.Received)
                .WriteByte(); // NOTE: 0 - None, 1 - Fame, 2 - Gift.
        }
    }
}
