using Destiny.Game;
using Destiny.Game.Maps;
using Destiny.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Destiny.Server
{
    public sealed class DataProvider
    {
        public SortedDictionary<int, Item> Items { get; private set; }
        public SortedDictionary<int, Equip> Equips { get; private set; }
        public SortedDictionary<int, Map> Maps { get; private set; }

        public DataProvider()
        {
            this.Items = new SortedDictionary<int, Item>();
            this.Equips = new SortedDictionary<int, Equip>();
            this.Maps = new SortedDictionary<int, Map>();
        }

        public void Load()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            this.LoadItems();
            this.LoadEquips();
            this.LoadMaps();

            sw.Stop();

            Logger.Write(LogLevel.Info, "Loaded data in {0}ms.", sw.ElapsedMilliseconds);
        }

        private void LoadItems()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Items.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        Item item = new Item(reader);

                        this.Items.Add(item.MapleID, item);
                    }
                }
            }
        }

        private void LoadEquips()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Equips.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        Equip equip = new Equip(reader);

                        this.Equips.Add(equip.MapleID, equip);
                    }
                }
            }
        }

        private void LoadMaps()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Maps.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        Map map = new Map(reader);

                        int portalsCount = reader.ReadInt32();

                        while (portalsCount-- > 0)
                        {
                            map.Portals.Add(new Portal(reader));
                        }

                        this.Maps.Add(map.MapleID, map);
                    }
                }
            }
        }
    }
}
