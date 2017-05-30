using reWZ;
using System.IO;

namespace WZ2BIN
{
    internal static class EquipExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            using (FileStream stream = File.Create(Path.Combine(outputPath, "Equips.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    using (WZFile file = new WZFile(Path.Combine(inputPath, "Character.wz"), WZVariant.GMS, true, WZReadSelection.None))
                    {
                        foreach (var category in file.MainDirectory)
                        {
                            if (category.Name.EndsWith(".img") || category.Name == "Afterimage" || category.Name == "Face" || category.Name == "Hair")
                            {
                                continue;
                            }

                            foreach (var node in category)
                            {
                                writer.Write(int.Parse(node.Name.Replace(".img", "")));
                                writer.Write((short)1); // NOTE: slotMax.
                                writer.Write(node["info"].GetInt("price"));
                                writer.Write(node["info"].GetByte("tuc"));
                                writer.Write(node["info"].GetShort("incSTR"));
                                writer.Write(node["info"].GetShort("incDEX"));
                                writer.Write(node["info"].GetShort("incINT"));
                                writer.Write(node["info"].GetShort("incLUK"));
                                writer.Write(node["info"].GetShort("incHP"));
                                writer.Write(node["info"].GetShort("incMP"));
                                writer.Write(node["info"].GetShort("incPAD"));
                                writer.Write(node["info"].GetShort("incMAD"));
                                writer.Write(node["info"].GetShort("incPDD"));
                                writer.Write(node["info"].GetShort("incMDD"));
                                writer.Write(node["info"].GetShort("incACC"));
                                writer.Write(node["info"].GetShort("incEVA"));
                                writer.Write(node["info"].GetShort("incHands"));
                                writer.Write(node["info"].GetShort("incSpeed"));
                                writer.Write(node["info"].GetShort("incJump"));
                            }
                        }
                    }
                }
            }
        }
    }
}