using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Game.Data
{
    public sealed class NpcDataProvider
    {
        private Dictionary<int, NpcData> mNpcs;

        public NpcDataProvider()
        {
            mNpcs = new Dictionary<int, NpcData>();
        }

        public void Load()
        {
            mNpcs.Clear();

            int count;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(Config.Instance.Binary, "Npcs.bin"))))
            {
                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    NpcData npc = new NpcData();
                    npc.Load(reader);
                    mNpcs.Add(npc.MapleID, npc);
                }
            }
        }

        public bool IsValidNpc(int mapleID)
        {
            return mNpcs.ContainsKey(mapleID);
        }

        public NpcData GetNpcData(int mapleID)
        {
            return mNpcs[mapleID];
        }
    }

    public sealed class NpcData
    {
        public int MapleID { get; set; }
        public bool IsMoving { get; set; }
        public bool IsMapleTV { get; set; }
        public bool IsGuildRank { get; set; }
        public int StorageCost { get; set; }
        public string Script { get; set; }

        public void Load(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.IsMoving = reader.ReadBoolean();
            this.IsMapleTV = reader.ReadBoolean();
            this.IsGuildRank = reader.ReadBoolean();
            this.StorageCost = reader.ReadInt32();
            this.Script = reader.ReadString();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.MapleID);
            writer.Write(this.IsMoving);
            writer.Write(this.IsMapleTV);
            writer.Write(this.IsGuildRank);
            writer.Write(this.StorageCost);
            writer.Write(this.Script);
        }
    }
}
