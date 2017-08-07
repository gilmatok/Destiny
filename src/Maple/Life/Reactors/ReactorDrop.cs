using Destiny.Data;

namespace Destiny.Maple.Life.Reactors
{
    public sealed class ReactorDrop
    {
        public int ItemID { get; private set; }
        public short MinQuantity { get; private set; }
        public short MaxQuantity { get; private set; }
        public ushort QuestID { get; private set; }
        public int Chance { get; private set; }

        public ReactorDrop(Datum datum)
        {
            this.ItemID = (int)datum["itemid"];
            this.MinQuantity = (short)(int)datum["minimum_quantity"];
            this.MaxQuantity = (short)(int)datum["maximum_quantity"];
            this.QuestID = (ushort)(int)datum["questid"];
            this.Chance = (int)datum["chance"];
        }
    }
}
