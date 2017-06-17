using Destiny;
using Destiny.Maple.Data;
using reWZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WZ2BIN
{
    internal static class ItemExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            List<ItemData> items = new List<ItemData>();

            using (WZFile file = new WZFile(Path.Combine(inputPath, "Item.wz"), WZVariant.GMS, true, WZReadSelection.None))
            {
                foreach (var category in file.MainDirectory)
                {
                    if (category.Name == "Pet" || category.Name == "Special")
                    {
                        continue;
                    }

                    foreach (var subcategory in category)
                    {
                        foreach (var node in subcategory)
                        {
                            ItemData item = new ItemData();

                            item.MapleID = int.Parse(node.Name);
                            item.MaxSlotQuantity = (short)node["info"].GetInt("slotMax", 1);
                            item.SalePrice = node["info"].GetInt("price");

                            items.Add(item);
                        }
                    }
                }
            }

            items = items.OrderBy(i => i.MapleID).ToList();

            using (FileStream stream = File.Create(Path.Combine(outputPath, "Items.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(items.Count);

                    foreach (ItemData item in items)
                    {
                        item.Save(writer);
                    }
                }
            }

            Logger.Write(LogLevel.Info, "Exported {0} items.", items.Count);

            items.Clear();
        }
    }
}