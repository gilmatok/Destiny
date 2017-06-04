using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server.Data
{
    public sealed class AbilityDataProvider
    {
        private Dictionary<byte, Dictionary<byte, AbilityData>> mAbilities;

        public AbilityDataProvider()
        {
            mAbilities = new Dictionary<byte, Dictionary<byte, AbilityData>>();
        }

        public void Load()
        {
            mAbilities.Clear();

            int count;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(Config.Instance.Binary, "Abilities.bin"))))
            {
                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    AbilityData Ability = new AbilityData();
                    Ability.Load(reader);

                    if (!mAbilities.ContainsKey(Ability.Identifier))
                    {
                        mAbilities.Add(Ability.Identifier, new Dictionary<byte, AbilityData>());
                    }

                    mAbilities[Ability.Identifier].Add(Ability.Level, Ability);
                }
            }
        }
    }

    public sealed class AbilityData
    {
        public byte Identifier { get; set; }
        public byte Level { get; set; }
        public ushort Duration { get; set; }
        public byte MPCost { get; set; }
        public int Parameter1 { get; set; }
        public int Parameter2 { get; set; }
        public byte Chance { get; set; }
        public byte TargetCount { get; set; }
        public ushort Cooldown { get; set; }
        public short LeftTopX { get; set; }
        public short LeftTopY { get; set; }
        public short RightBottomX { get; set; }
        public short RightBottomY { get; set; }
        public byte HPLimitPercent { get; set; }
        public ushort SummonLimit { get; set; }
        public byte SummonEffect { get; set; }
        public List<int> SummonIdentifiers { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write(Level);
            pWriter.Write(Duration);
            pWriter.Write(MPCost);
            pWriter.Write(Parameter1);
            pWriter.Write(Parameter2);
            pWriter.Write(Chance);
            pWriter.Write(TargetCount);
            pWriter.Write(Cooldown);
            pWriter.Write(LeftTopX);
            pWriter.Write(LeftTopY);
            pWriter.Write(RightBottomX);
            pWriter.Write(RightBottomY);
            pWriter.Write(HPLimitPercent);
            pWriter.Write(SummonLimit);
            pWriter.Write(SummonEffect);

            pWriter.Write(SummonIdentifiers.Count);
            SummonIdentifiers.ForEach(i => pWriter.Write(i));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadByte();
            Level = pReader.ReadByte();
            Duration = pReader.ReadUInt16();
            MPCost = pReader.ReadByte();
            Parameter1 = pReader.ReadInt32();
            Parameter2 = pReader.ReadInt32();
            Chance = pReader.ReadByte();
            TargetCount = pReader.ReadByte();
            Cooldown = pReader.ReadUInt16();
            LeftTopX = pReader.ReadInt16();
            LeftTopY = pReader.ReadInt16();
            RightBottomX = pReader.ReadInt16();
            RightBottomY = pReader.ReadInt16();
            HPLimitPercent = pReader.ReadByte();
            SummonLimit = pReader.ReadUInt16();
            SummonEffect = pReader.ReadByte();

            int summonsCount = pReader.ReadInt32();
            SummonIdentifiers = new List<int>(summonsCount);
            while (summonsCount-- > 0) SummonIdentifiers.Add(pReader.ReadInt32());
        }
    }
}
