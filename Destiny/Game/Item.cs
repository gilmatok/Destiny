using Destiny.Core.IO;
using Destiny.Utility;
using System;
using System.IO;

namespace Destiny.Game
{
    public class Item
    {
        public int Identifier { get; private set; }
        public ItemFlags Flags { get; private set; }
        public int Price { get; private set; }
        public ushort MaxSlotQuantity { get; private set; }
        public byte MaxPossessionCount { get; private set; }
        public byte MinLevel { get; private set; }
        public byte MaxLevel { get; private set; }
        public int Experience { get; private set; }
        public byte MakerLevel { get; private set; }
        public int Money { get; private set; }
        public int StateChangeItem { get; private set; }
        public int NPC { get; private set; }

        public short Slot { get; private set; }
        public short Quantity { get; private set; }
        public DateTime Expiration { get; private set; }

        public ItemType Type
        {
            get
            {
                return (ItemType)(this.Identifier / 1000000);
            }
        }

        public Item(DatabaseQuery query)
        {
            this.Identifier = query.GetInt("item_identifier");
            this.Slot = query.GetShort("slot");
            this.Quantity = query.GetShort("quantity");
            this.Expiration = query.GetDateTime("expiration");
        }

        public Item(BinaryReader reader)
        {
            this.Identifier = reader.ReadInt32();
            this.Flags = (ItemFlags)reader.ReadUInt16();
            this.Price = reader.ReadInt32();
            this.MaxSlotQuantity = reader.ReadUInt16();
            this.MaxPossessionCount = reader.ReadByte();
            this.MinLevel = reader.ReadByte();
            this.MaxLevel = reader.ReadByte();
            this.Experience = reader.ReadInt32();
            this.MakerLevel = reader.ReadByte();
            this.Money = reader.ReadInt32();
            this.StateChangeItem = reader.ReadInt32();
            this.NPC = reader.ReadInt32();
        }

        public virtual void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(2)
                .WriteInt(this.Identifier)
                .WriteBool(false)
                .WriteLong() // TODO: Expiration.
                .WriteShort(this.Quantity)
                .WriteString(string.Empty) // NOTE: Creator.
                .WriteShort(); // NOTE: Flags.
        }
    }
}
