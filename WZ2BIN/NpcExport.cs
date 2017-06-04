using Destiny;
using Destiny.Server.Data;
using reWZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WZ2BIN
{
    internal static class NpcExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            List<NpcData> npcs = new List<NpcData>();

            using (WZFile file = new WZFile(Path.Combine(inputPath, "Npc.wz"), WZVariant.GMS, true, WZReadSelection.None))
            {
                foreach (var npcNode in file.MainDirectory)
                {
                    NpcData npc = new NpcData();

                    npc.MapleID = npcNode.GetID();
                    npc.IsMoving = npcNode.HasChild("move");
                    npc.IsMapleTV = npcNode["info"].HasChild("MapleTV");
                    npc.IsGuildRank = npcNode["info"].HasChild("guildRank");
                    npc.StorageCost = npcNode["info"].GetInt("trunkPut");

                    if (npcNode.HasChild("script"))
                    {
                        npc.Script = npcNode["script"]["0"].GetString("script");
                    }
                    else
                    {
                        npc.Script = string.Empty;
                    }

                    npcs.Add(npc);
                }
            }

            npcs = npcs.OrderBy(m => m.MapleID).ToList();

            using (FileStream stream = File.Create(Path.Combine(outputPath, "Npcs.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(npcs.Count);

                    foreach (NpcData npc in npcs)
                    {
                        npc.Save(writer);
                    }
                }
            }

            Logger.Write(LogLevel.Info, "Exported {0} npcs.", npcs.Count);

            npcs.Clear();
        }
    }
}
