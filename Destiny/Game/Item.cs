using Destiny.Core.IO;
using System;

namespace Destiny.Game
{
    public class Item
    {
        public int MapleID { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public virtual void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.MapleID)
                .WriteBool(false); // TODO: If SN is not -1.

            if (false) // TODO: If SN is not -1.
            {
                // TODO: Write SN.
            }

            oPacket.WriteDateTime(this.Expiration);
        }
    }
}
