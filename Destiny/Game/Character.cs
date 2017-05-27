using Destiny.Network;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Game
{
    public sealed class Character
    {
        public MapleClient Client { get; private set; }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public byte WorldID { get; set; }
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

        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }

        public Character(MapleClient client, DatabaseQuery query)
        {
            this.Client = client;

            this.ID = query.GetInt("character_id");
            this.AccountID = query.GetInt("account_id");
            this.WorldID = query.GetByte("world_id");
            this.Name = query.GetString("name");
            this.Gender = (Gender)query.GetByte("gender");
            this.Skin = query.GetByte("skin");
            this.Face = query.GetInt("face");
            this.Hair = query.GetInt("hair");
            this.Level = query.GetByte("level");
            this.Job = (Job)query.GetShort("job");
            this.Strength = query.GetShort("strength");
            this.Dexterity = query.GetShort("dexterity");
            this.Intelligence = query.GetShort("intelligence");
            this.Luck = query.GetShort("luck");
            this.Health = query.GetShort("health");
            this.MaxHealth = query.GetShort("max_health");
            this.Mana = query.GetShort("mana");
            this.MaxMana = query.GetShort("max_mana");
            this.AbilityPoints = query.GetShort("ability_points");
            this.SkillPoints = query.GetShort("skill_points");
            this.Experience = query.GetInt("experience");
            this.Fame = query.GetShort("fame");

            using (DatabaseQuery itemQuery = Database.Query("SELECT * FROM `items` WHERE `character_id` = @character_id", new MySqlParameter("character_id", this.ID)))
            {
                this.Items = new CharacterItems(this, itemQuery);
            }

            using (DatabaseQuery skillQuery = null)
            {
                this.Skills = new CharacterSkills(this, skillQuery);
            }

            using (DatabaseQuery questQuery = null)
            {
                this.Quests = new CharacterQuests(this, questQuery);
            }
        }

        public void Save()
        {

        }
    }
}
