using Destiny;
using Destiny.Game.Data;
using reWZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WZ2BIN
{
    internal static class EquipExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            List<EquipData> equips = new List<EquipData>();

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
                        EquipData equip = new EquipData();

                        equip.MapleID = node.GetID();
                        equip.MaxSlotQuantity = 1;
                        equip.SalePrice = node["info"].GetInt("price");
                        equip.Slots = (byte)node["info"].GetInt("tuc");
                        equip.Strength = (short)node["info"].GetInt("incSTR");
                        equip.Dexterity = (short)node["info"].GetInt("incDEX");
                        equip.Intelligence = (short)node["info"].GetInt("incINT");
                        equip.Luck = (short)node["info"].GetInt("incLUK");
                        equip.Health = (short)node["info"].GetInt("incHP");
                        equip.Mana = (short)node["info"].GetInt("incMP");
                        equip.WeaponAttack = (short)node["info"].GetInt("incPAD");
                        equip.MagicAttack = (short)node["info"].GetInt("incMAD");
                        equip.WeaponDefense = (short)node["info"].GetInt("incPDD");
                        equip.MagicDefense = (short)node["info"].GetInt("incMDD");
                        equip.Accuracy = (short)node["info"].GetInt("incACC");
                        equip.Avoidability = (short)node["info"].GetInt("incEVA");
                        equip.Hands = (short)node["info"].GetInt("incHands");
                        equip.Speed = (short)node["info"].GetInt("incSpeed");
                        equip.Jump = (short)node["info"].GetInt("incJump");

                        equips.Add(equip);
                    }
                }
            }

            equips = equips.OrderBy(e => e.MapleID).ToList();

            using (FileStream stream = File.Create(Path.Combine(outputPath, "Equips.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(equips.Count);

                    foreach (EquipData equip in equips)
                    {
                        equip.Save(writer);
                    }
                }
            }

            Logger.Write(LogLevel.Info, "Exported {0} equips.", equips.Count);

            equips.Clear();
        }
    }
}