using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server.Data
{
    public sealed class ItemDataProvider
    {
        private Dictionary<int, ItemData> mItems;

        public ItemDataProvider()
        {
            mItems = new Dictionary<int, ItemData>();
        }

        public void Load()
        {
            mItems.Clear();

            int count;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(Config.Instance.Binary, "Items.bin"))))
            {
                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    ItemData item = new ItemData();
                    item.Load(reader);
                    mItems.Add(item.MapleID, item);
                }
            }
        }

        public bool IsValidItem(int mapleID)
        {
            return mItems.ContainsKey(mapleID);
        }

        public ItemData GetItemData(int mapleID)
        {
            return mItems[mapleID];
        }
    }

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
