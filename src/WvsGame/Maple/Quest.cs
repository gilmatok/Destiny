using Destiny.Data;
using Destiny.Data;
using Destiny.Maple.Data;
using System.Collections.Generic;

namespace Destiny.Maple
{
    public class Quest
    {
        public ushort MapleID { get; private set; }
        public ushort NextQuestID { get; private set; }
        public sbyte Area { get; private set; }
        public byte MinimumLevel { get; private set; }
        public byte MaximumLevel { get; private set; }
        public short PetCloseness { get; private set; }
        public sbyte TamingMobLevel { get; private set; }
        public int RepeatWait { get; private set; }
        public short Fame { get; private set; }
        public int TimeLimit { get; private set; }
        public bool AutoStart { get; private set; }
        public bool SelectedMob { get; private set; }

        public List<ushort> PreRequiredQuests { get; private set; }
        public List<ushort> PostRequiredQuests { get; private set; }
        public Dictionary<int, short> PreRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredKills { get; private set; }
        public List<Job> ValidJobs { get; private set; }

        // Rewards (Start, End)
        public int[] ExperienceReward { get; set; }
        public int[] MesoReward { get; set; }
        public int[] PetClosenessReward { get; set; }
        public bool[] PetSpeedReward { get; set; }
        public int[] FameReward { get; set; }
        public int[] PetSkillReward { get; set; }
        public Dictionary<int, short> PreItemRewards { get; private set; }
        public Dictionary<int, short> PostItemRewards { get; private set; }
        public Dictionary<Skill, Job> PreSkillRewards { get; set; }
        public Dictionary<Skill, Job> PostSkillRewards { get; set; }

        public Quest NextQuest
        {
            get
            {
                return this.NextQuestID > 0 ? DataProvider.Quests[this.NextQuestID] : null;
            }
        }

        public byte Flags
        {
            get
            {
                byte flags = 0;

                if (this.AutoStart) flags |= (byte)QuestFlags.AutoStart;
                if (this.SelectedMob) flags |= (byte)QuestFlags.SelectedMob;

                return flags;
            }
        }

        public Quest(Datum datum)
        {
            this.MapleID = (ushort)datum["questid"];
            this.NextQuestID = (ushort)datum["next_quest"];
            this.Area = (sbyte)datum["quest_area"];
            this.MinimumLevel = (byte)datum["min_level"];
            this.MaximumLevel = (byte)datum["max_level"];
            this.PetCloseness = (short)datum["pet_closeness"];
            this.TamingMobLevel = (sbyte)datum["taming_mob_level"];
            this.RepeatWait = (int)datum["repeat_wait"];
            this.Fame = (short)datum["fame"];
            this.TimeLimit = (int)datum["time_limit"];
            this.AutoStart = datum["flags"].ToString().Contains("auto_start");
            this.SelectedMob = datum["flags"].ToString().Contains("selected_mob");

            this.PreRequiredQuests = new List<ushort>();
            this.PostRequiredQuests = new List<ushort>();
            this.PreRequiredItems = new Dictionary<int, short>();
            this.PostRequiredItems = new Dictionary<int, short>();
            this.PostRequiredKills = new Dictionary<int, short>();

            this.ExperienceReward = new int[2];
            this.MesoReward = new int[2];
            this.PetClosenessReward = new int[2];
            this.PetSpeedReward = new bool[2];
            this.FameReward = new int[2];
            this.PetSkillReward = new int[2];

            this.PreItemRewards = new Dictionary<int, short>();
            this.PostItemRewards = new Dictionary<int, short>();
            this.PreSkillRewards = new Dictionary<Skill, Job>();
            this.PostSkillRewards = new Dictionary<Skill, Job>();

            this.ValidJobs = new List<Job>();
        }
    }
}
