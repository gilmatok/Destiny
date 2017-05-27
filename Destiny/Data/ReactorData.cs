using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class ReactorData
    {
        [Flags]
        public enum EReactorFlags
        {
            None = 0 << 0,
            Activate_By_Touch = 1 << 0,
            ActivateByTouch = 1 << 0,
            Remove_In_Field_Set = 1 << 1,
            RemoveInFieldSet = 1 << 1
        }

        public sealed class ReactorStateData
        {
            public enum EReactorStateType : byte
            {
                None = 0,
                Plain_Advance_State = 1,
                PlainAdvanceState = 1,
                Hit_From_Left = 2,
                HitFromLeft = 2,
                Hit_From_Right = 3,
                HitFromRight = 3,
                Hit_By_Skill = 4,
                HitBySkill = 4,
                No_Clue = 5,
                No_Clue2 = 6,
                Hit_By_Item = 7,
                HitByItem = 7
            }


            public byte State { get; set; }
            public EReactorStateType Type { get; set; }
            public int Timeout { get; set; }
            public int ItemIdentifier { get; set; }
            public byte Quantity { get; set; }
            public short LeftTopX { get; set; }
            public short LeftTopY { get; set; }
            public short RightBottomX { get; set; }
            public short RightBottomY { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(State);
                pWriter.Write((byte)Type);
                pWriter.Write(Timeout);
                pWriter.Write(ItemIdentifier);
                pWriter.Write(Quantity);
                pWriter.Write(LeftTopX);
                pWriter.Write(LeftTopY);
                pWriter.Write(RightBottomX);
                pWriter.Write(RightBottomY);
            }

            public void Load(BinaryReader pReader)
            {
                State = pReader.ReadByte();
                Type = (EReactorStateType)pReader.ReadByte();
                Timeout = pReader.ReadInt32();
                ItemIdentifier = pReader.ReadInt32();
                Quantity = pReader.ReadByte();
                LeftTopX = pReader.ReadInt16();
                LeftTopY = pReader.ReadInt16();
                RightBottomX = pReader.ReadInt16();
                RightBottomY = pReader.ReadInt16();
            }
        }

        public sealed class ReactorDropData
        {
            [Flags]
            public enum EReactorDropFlags
            {
                None = 0 << 0,
                Is_Mesos = 1 << 0,
                IsMesos = 1 << 0
            }

            public EReactorDropFlags Flags { get; set; }
            public int ItemIdentifier { get; set; }
            public int Minimum { get; set; }
            public int Maximum { get; set; }
            public ushort QuestIdentifier { get; set; }
            public int Chance { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(ItemIdentifier);
                pWriter.Write(Minimum);
                pWriter.Write(Maximum);
                pWriter.Write(QuestIdentifier);
                pWriter.Write(Chance);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EReactorDropFlags)pReader.ReadByte();
                ItemIdentifier = pReader.ReadInt32();
                Minimum = pReader.ReadInt32();
                Maximum = pReader.ReadInt32();
                QuestIdentifier = pReader.ReadUInt16();
                Chance = pReader.ReadInt32();
            }
        }


        public int Identifier { get; set; }
        public EReactorFlags Flags { get; set; }
        public int LinkIdentifier { get; set; }
        public List<ReactorStateData> States { get; set; }
        public List<ReactorDropData> Drops { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((byte)Flags);
            pWriter.Write(LinkIdentifier);

            pWriter.Write(States.Count);
            States.ForEach(s => s.Save(pWriter));

            pWriter.Write(Drops.Count);
            Drops.ForEach(d => d.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (EReactorFlags)pReader.ReadByte();
            LinkIdentifier = pReader.ReadInt32();

            int statesCount = pReader.ReadInt32();
            States = new List<ReactorStateData>(statesCount);
            while (statesCount-- > 0)
            {
                ReactorStateData state = new ReactorStateData();
                state.Load(pReader);
                States.Add(state);
            }

            int dropsCount = pReader.ReadInt32();
            Drops = new List<ReactorDropData>(dropsCount);
            while (dropsCount-- > 0)
            {
                ReactorDropData drop = new ReactorDropData();
                drop.Load(pReader);
                Drops.Add(drop);
            }
        }
    }
}
