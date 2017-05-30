using reWZ;
using System.IO;

namespace WZ2BIN
{
    internal static class MapExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            using (FileStream stream = File.Create(Path.Combine(outputPath, "Maps.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    using (WZFile file = new WZFile(Path.Combine(inputPath, "Map.wz"), WZVariant.GMS, true, WZReadSelection.None))
                    {
                        foreach (var category in file.MainDirectory["Map"])
                        {
                            if (category.Name == "AreaCode.img")
                            {
                                continue;
                            }

                            foreach (var node in category)
                            {
                                writer.Write(int.Parse(node.Name.Replace(".img", "")));
                                writer.Write(node["info"].GetInt("returnMap"));
                                writer.Write(node["info"].GetInt("forcedReturn"));

                                if (node.HasChild("portal"))
                                {
                                    writer.Write(node["portal"].ChildCount);

                                    foreach (var portalNode in node["portal"])
                                    {
                                        writer.Write(byte.Parse(portalNode.Name));
                                        writer.Write(portalNode.GetString("pn"));
                                        writer.Write(portalNode.GetInt("tm"));
                                        writer.Write(portalNode.GetString("tn"));
                                        writer.Write(portalNode.GetString("script"));
                                        writer.Write(portalNode.GetShort("x"));
                                        writer.Write(portalNode.GetShort("y"));
                                    }
                                }
                                else
                                {
                                    writer.Write(0);
                                }

                                if (node.HasChild("life"))
                                {
                                    writer.Write(node["life"].ChildCount);

                                    foreach (var lifeNode in node["life"])
                                    {
                                        string type = lifeNode.GetString("type");

                                        writer.Write(type);
                                        writer.Write(lifeNode.GetInt("id"));
                                        writer.Write(lifeNode.GetShort("x"));
                                        writer.Write(lifeNode.GetShort("cy"));
                                        writer.Write(lifeNode.GetShort("fh"));
                                        writer.Write(lifeNode.GetByte("f"));
                                        writer.Write(lifeNode.GetShort("rx0"));
                                        writer.Write(lifeNode.GetShort("rx1"));
                                        writer.Write(lifeNode.GetByte("hide"));
                                        writer.Write(lifeNode.GetInt("mobTime"));
                                    }
                                }
                                else
                                {
                                    writer.Write(0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}