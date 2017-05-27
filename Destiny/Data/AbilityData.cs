using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
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
        public uint Cooldown { get; set; }
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
            Cooldown = pReader.ReadUInt32();
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
