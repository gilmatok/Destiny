using System.Collections.Generic;
using System.IO;

namespace Destiny.Game
{
    public sealed class ReactorState
    {
        public byte State { get; private set; }
        public ReactorStateType Type { get; private set; }
        public int Timeout { get; private set; }
        public int ItemID { get; private set; }
        public byte Quantity { get; private set; }
        public short LeftTopX { get; private set; }
        public short LeftTopY { get; private set; }
        public short RightBottomX { get; private set; }
        public short RightBottomY { get; private set; }

        public ReactorState(BinaryReader reader)
        {
            this.State = reader.ReadByte();
            this.Type = (ReactorStateType)reader.ReadByte();
            this.Timeout = reader.ReadInt32();
            this.ItemID = reader.ReadInt32();
            this.Quantity = reader.ReadByte();
            this.LeftTopX = reader.ReadInt16();
            this.LeftTopY = reader.ReadInt16();
            this.RightBottomX = reader.ReadInt16();
            this.RightBottomY = reader.ReadInt16();
        }
    }

    public sealed class ReactorDrop
    {
        public ReactorDropFlags Flags { get; private set; }
        public int ItemID { get; private set; }
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }
        public ushort QuestID { get; private set; }
        public int Chance { get; private set; }

        public ReactorDrop(BinaryReader reader)
        {
            this.Flags = (ReactorDropFlags)reader.ReadByte();
            this.ItemID = reader.ReadInt32();
            this.Minimum = reader.ReadInt32();
            this.Maximum = reader.ReadInt32();
            this.QuestID = reader.ReadUInt16();
            this.Chance = reader.ReadInt32();
        }
    }

    public sealed class Reactor
    {
        public int MapleID { get; private set; }
        public ReactorFlags Flags { get; private set; }
        public int LinkID { get; private set; }
        public List<ReactorState> States { get; private set; }
        public List<ReactorDrop> Drops { get; private set; }

        public Reactor(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.Flags = (ReactorFlags)reader.ReadByte();
            this.LinkID = reader.ReadInt32();

            int statesCount = reader.ReadInt32();

            this.States = new List<ReactorState>(statesCount);

            while (statesCount-- > 0)
            {
                this.States.Add(new ReactorState(reader));
            }

            int dropsCount = reader.ReadInt32();

            this.Drops = new List<ReactorDrop>(dropsCount);

            while (dropsCount-- > 0)
            {
                this.Drops.Add(new ReactorDrop(reader));
            }
        }
    }
}
