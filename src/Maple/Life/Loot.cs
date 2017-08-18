using Destiny.Data;

namespace Destiny.Maple.Life
{
    public sealed class Loot
    {
        public int MapleID { get; private set; }
        public int MinimumQuantity { get; private set; }
        public int MaximumQuantity { get; private set; }
        public int QuestID { get; private set; }
        public int Chance { get; private set; }
        public bool IsMeso { get; private set; }

        public Loot(Datum datum)
        {
            this.MapleID = (int)datum["itemid"];
            this.MinimumQuantity = (int)datum["minimum_quantity"];
            this.MaximumQuantity = (int)datum["maximum_quantity"];
            this.QuestID = (int)datum["questid"];
            this.Chance = (int)datum["chance"];
            this.IsMeso = ((string)datum["flags"]).Contains("is_mesos");
        }
    }
}
