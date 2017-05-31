using System.IO;

namespace Destiny.Game.Data
{
    public class ItemData
    {
        public int MapleID { get; set; }
        public short MaxSlotQuantity { get; set; }
        public int SalePrice { get; set; }

        public virtual void Load(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.MaxSlotQuantity = reader.ReadInt16();
            this.SalePrice = reader.ReadInt32();
        }

        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(this.MapleID);
            writer.Write(this.MaxSlotQuantity);
            writer.Write(this.SalePrice);
        }
    }
}
