using Destiny;
using Destiny.Game;
using Destiny.Server.Data;
using reWZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WZ2BIN
{
    internal static class MapExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            List<MapData> maps = new List<MapData>();

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
                        MapData map = new MapData();

                        map.MapleID = node.GetID();
                        map.ReturnMapID = node["info"].GetInt("returnMap");
                        map.ForcedReturnMapID = node["info"].GetInt("forcedReturn");

                        map.Footholds = new List<MapFootholdData>();

                        if (node.HasChild("foothold"))
                        {
                            foreach (var footholdNode in node["foothold"])
                            {

                            }
                        }

                        map.Mobs = new List<MapMobSpawnData>();
                        map.Npcs = new List<MapNpcSpawnData>();

                        if (node.HasChild("life"))
                        {
                            foreach (var lifeNode in node["life"])
                            {
                                string type = lifeNode.GetString("type");
                                int mapleID = lifeNode.GetInt("id");
                                short foothold = (short)lifeNode.GetInt("fh");
                                Point position = new Point(lifeNode.GetInt("x"), lifeNode.GetInt("y"));
                                bool flip = lifeNode.GetInt("f") == 1;
                                bool hide = lifeNode.GetInt("hide") == 1;

                                if (type == "m")
                                {
                                    MapMobSpawnData mob = new MapMobSpawnData();

                                    mob.MapleID = mapleID;
                                    mob.Foothold = foothold;
                                    mob.Positon = position;
                                    mob.Flip = flip;
                                    mob.Hide = hide;
                                    mob.RespawnTime = lifeNode.GetInt("mobTime");

                                    map.Mobs.Add(mob);
                                }
                                else if (type == "n")
                                {
                                    MapNpcSpawnData npc = new MapNpcSpawnData();

                                    npc.MapleID = mapleID;
                                    npc.Foothold = foothold;
                                    npc.Positon = position;
                                    npc.Flip = flip;
                                    npc.Hide = hide;
                                    npc.MinimumClickX = (short)lifeNode.GetInt("rx0");
                                    npc.MaximumClickX = (short)lifeNode.GetInt("rx1");

                                    map.Npcs.Add(npc);
                                }
                            }
                        }

                        map.Portals = new List<MapPortalData>();

                        if (node.HasChild("portal"))
                        {
                            foreach (var portalNode in node["portal"])
                            {
                                MapPortalData portal = new MapPortalData();

                                portal.ID = byte.Parse(portalNode.Name);
                                portal.Label = portalNode.GetString("pn");
                                portal.DestinationMap = portalNode.GetInt("tm");
                                portal.DestinationLabel = portalNode.GetString("tn");
                                portal.Script = portalNode.GetString("script");
                                portal.Position = new Point(portalNode.GetInt("x"), portalNode.GetInt("y"));

                                map.Portals.Add(portal);
                            }
                        }

                        map.Reactors = new List<MapReactorData>();

                        if (node.HasChild("reactor"))
                        {
                            foreach (var reactorNode in node["reactor"])
                            {

                            }
                        }

                        map.Seats = new List<MapSeatData>();

                        if (node.HasChild("seat"))
                        {
                            foreach (var seatNode in node["seat"])
                            {

                            }
                        }

                        maps.Add(map);
                    }
                }
            }

            maps = maps.OrderBy(m => m.MapleID).ToList();

            using (FileStream stream = File.Create(Path.Combine(outputPath, "Maps.bin")))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(maps.Count);

                    foreach (MapData map in maps)
                    {
                        map.Save(writer);
                    }
                }
            }

            Logger.Write(LogLevel.Info, "Exported {0} maps.", maps.Count);

            maps.Clear();
        }
    }
}