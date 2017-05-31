using Destiny.Core.IO;
using Destiny.Game.Data;
using Destiny.Server;
using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public class Item
    {
        public int MapleID { get; private set; }
        public ItemData Data { get; private set; }
        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public Item(int mapleID)
        {
            this.MapleID = mapleID;
            this.Data = MasterServer.Instance.Data.Items[this.MapleID];
        }

        public Item(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("item_identifier");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }
    
        public virtual void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(2)
                .WriteInt(this.MapleID)
                .WriteBool(false)
                .WriteLong() // TODO: Expiration.
                .WriteShort(this.Quantity)
                .WriteMapleString(string.Empty) // NOTE: Creator.
                .WriteShort(); // NOTE: Flags.
        }
    }
}
