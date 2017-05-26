using MongoDB.Bson;

namespace Destiny.Game
{
    public sealed class Character
    {
        public ObjectId _id;

        public int CharacterId { get; set; }
        public int AccountId { get; set; }
        public byte WorldId { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public byte Skin { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }
        public byte Level { get; set; }
        public Job Job { get; set; }
        public short Strength { get; set; }
        public short Dexterity { get; set; }
        public short Intelligence { get; set; }
        public short Luck { get; set; }
        public short Health { get; set; }
        public short MaxHealth { get; set; }
        public short Mana { get; set; }
        public short MaxMana { get; set; }
        public short AbilityPoints { get; set; }
        public short SkillPoints { get; set; }
        public int Experience { get; set; }
        public short Fame { get; set; }

        public void Save()
        {

        }
    }
}
