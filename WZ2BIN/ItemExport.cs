using reWZ;
using System.IO;

namespace WZ2BIN
{
    internal static class ItemExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            using (FileStream stream = File.Create(Path.Combine(outputPath, "Items.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
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
                                    writer.Write(int.Parse(node.Name));
                                    writer.Write(node["info"].GetShort("slotMax", 1));
                                    writer.Write(node["info"].GetInt("price"));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
