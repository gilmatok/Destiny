using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class PartyMember
    {
        public Party Party { get; private set; }

        private int id;
        private string name;
        private byte level;
        private Job job;
        private int map;
        private int channel;

        private bool Assigned { get; set; }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public byte Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public Job Job
        {
            get
            {
                return job;
            }
            set
            {
                job = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public int Map
        {
            get
            {
                return map;
            }
            set
            {
                map = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public int Channel
        {
            get
            {
                return channel;
            }
            set
            {
                channel = value;

                if (this.Assigned)
                {
                    this.Party.Update();
                }
            }
        }

        public PartyMember(Party party, Character character)
        {
            this.Party = party;

            this.ID = character.ID;
            this.Name = character.Name;
            this.Level = character.Level;
            this.Job = character.Job;
            this.Map = character.Map.MapleID;
            this.Channel = character.Client.ChannelID;

            this.Assigned = true;
        }
    }
}
