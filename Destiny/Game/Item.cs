using Destiny.Core.IO;
using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public class Item
    {
        public int MapleID { get; private set; }
        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public Item(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("maple_id");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }

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
