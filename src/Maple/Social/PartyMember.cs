using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class PartyMember
    {
        public Party Party { get; set; }

        public int ID { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public Job Job { get; set; }
        public int Map { get; set; }
        public int Channel { get; set; }
        public bool Expelled { get; set; }

        private Character character;
        public Character Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;

                if (value == null)
                {
                    this.Map = 999999999;
                    this.Channel = -2;
                }
                else
                {
                    this.Map = character.Map.MapleID;
                    this.Channel = character.Client.ChannelID;
                }

                this.Party.Update();
            }
        }

        public PartyMember(Character character)
        {
            this.ID = character.ID;
            this.Name = character.Name;
            this.Level = character.Level;
            this.Job = character.Job;
            this.Map = character.Map.MapleID;
            this.Channel = character.Client.ChannelID;
            this.character = character;
        }
    }
}