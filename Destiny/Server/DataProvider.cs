using Destiny.Game;
using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server
{
    public sealed class DataProvider
    {
        public Dictionary<int, Npc> Npcs { get; private set; }
        public Dictionary<int, Reactor> Reactors { get; private set; }

        public DataProvider()
        {
            this.Npcs = new Dictionary<int, Npc>();
            this.Reactors = new Dictionary<int, Reactor>();
        }

        public void Load()
        {
            Logger.Initializer("Loading Maple Data", () =>
            {
                using (FileStream stream = File.Open(Config.Instance.Binary, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        this.LoadAbilities(reader);
                        this.LoadSkills(reader);
                        this.LoadNpcs(reader);
                        this.LoadReactors(reader);
                        this.LoadMobs(reader);
                        this.LoadQuests(reader);
                        this.LoadItems(reader);
                        this.LoadMaps(reader);
                    }
                }
            });
        }

        private void LoadAbilities(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadByte();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadByte();

                int summonCount = reader.ReadInt32();

                while (summonCount-- > 0)
                {
                    reader.ReadInt32();
                }
            }
        }

        private void LoadSkills(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt32();
                reader.ReadUInt16();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadUInt16();
            }
        }

        private void LoadNpcs(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {
                Npc npc = new Npc(reader);

                this.Npcs.Add(npc.MapleID, npc);
            }
        }

        private void LoadReactors(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {
                Reactor reactor = new Reactor(reader);

                this.Reactors.Add(reactor.MapleID, reactor);
            }
        }

        private void LoadMobs(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {

            }
        }

        private void LoadQuests(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {

            }
        }

        private void LoadItems(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {

            }
        }

        private void LoadMaps(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            while (count-- > 0)
            {

            }
        }
    }
}
