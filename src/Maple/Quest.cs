using Destiny.Data;
using Destiny.Maple.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<ushort, QuestStatus> PreRequiredQuests { get; private set; }
        public Dictionary<ushort, QuestStatus> PostRequiredQuests { get; private set; }
        public Dictionary<int, short> PreRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredKills { get; private set; }
        public List<Job> ValidJobs { get; private set; }

        // Rewards (Start, End)
        public Tuple<int, int> ExperienceReward { get; private set; }
        public Tuple<int, int> MesoReward { get; private set; }
        public Tuple<int, int> PetClosenessReward { get; private set; }
        public Tuple<bool, bool> PetSpeedReward { get; private set; }
        public Tuple<int, int> FameReward { get; private set; }
        public Tuple<int, int> PetSkillReward { get; private set; } //TODO: Use enum instead of int
        public Tuple<int, int> BuffReward { get; private set; } //TODO: Use enum instead of int

        public List<Tuple<int, short, Gender>> PreItemRewards { get; private set; }
        public List<Tuple<int, short, Gender>> PostItemRewards { get; private set; }
        public Dictionary<Skill, Job> PreSkillRewards { get; private set; }
        public Dictionary<Skill, Job> PostSkillRewards { get; private set; }

        public Quest CachedReference
        {
            get
            {
                return DataProvider.Quests[this.MapleID];
            }
        }

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
        
        public Quest(ushort mapleID)
        {
            this.MapleID = mapleID;
            this.NextQuestID = CachedReference.NextQuestID;
            this.Area = CachedReference.Area;
            this.MinimumLevel = CachedReference.MinimumLevel;
            this.MaximumLevel = CachedReference.MaximumLevel;
            this.PetCloseness = CachedReference.PetCloseness;
            this.TamingMobLevel = CachedReference.TamingMobLevel;
            this.RepeatWait = CachedReference.RepeatWait;
            this.Fame = CachedReference.Fame;
            this.TimeLimit = CachedReference.TimeLimit;
            this.AutoStart = CachedReference.AutoStart;
            this.SelectedMob = CachedReference.SelectedMob;

            this.PreRequiredQuests = CachedReference.PreRequiredQuests;
            this.PostRequiredQuests = CachedReference.PostRequiredQuests;
            this.PreRequiredItems = CachedReference.PreRequiredItems;
            this.PostRequiredItems = CachedReference.PostRequiredItems;
            this.PostRequiredKills = CachedReference.PostRequiredKills;

            this.ExperienceReward = CachedReference.ExperienceReward;
            this.MesoReward = CachedReference.MesoReward;
            this.PetClosenessReward = CachedReference.PetClosenessReward;
            this.PetSpeedReward = CachedReference.PetSpeedReward;
            this.FameReward = CachedReference.FameReward;
            this.PetSkillReward = CachedReference.PetSkillReward;
            this.BuffReward = CachedReference.BuffReward;

            this.PreItemRewards = CachedReference.PreItemRewards;
            this.PostItemRewards = CachedReference.PostItemRewards;
            this.PreSkillRewards = CachedReference.PreSkillRewards;
            this.PostSkillRewards = CachedReference.PostSkillRewards;

            this.ValidJobs = CachedReference.ValidJobs;
        }

        public Quest(Datum datum, Datums requests, Datums rewards, Datums jobs)
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

            this.PreRequiredQuests  = requests.Where(x => (string)x["request_type"] == "quest" && (string)x["quest_state"] == "start")
                                              .Select(x => new { objectid = (int)x["objectid"], quantity = (short)x["quantity"] })
                                              .ToDictionary(x => (ushort)x.objectid, x => (QuestStatus)(byte)x.quantity);
            this.PostRequiredQuests = requests.Where(x => (string)x["request_type"] == "quest" && (string)x["quest_state"] == "end")
                                              .Select(x => new { objectid = (int)x["objectid"], quantity = (short)x["quantity"] })
                                              .ToDictionary(x => (ushort)x.objectid, x => (QuestStatus)(byte)x.quantity);
            this.PreRequiredItems   = requests.Where(x => (string)x["request_type"] == "item" && (string)x["quest_state"] == "start")
                                              .Select(x => new { objectid = (int)x["objectid"], quantity = (short)x["quantity"] })
                                              .ToDictionary(x => x.objectid, x => x.quantity);
            this.PostRequiredItems  = requests.Where(x => (string)x["request_type"] == "item" && (string)x["quest_state"] == "end")
                                              .Select(x => new { objectid = (int)x["objectid"], quantity = (short)x["quantity"] })
                                              .ToDictionary(x => x.objectid, x => x.quantity);
            this.PostRequiredKills  = requests.Where(x => (string)x["request_type"] == "mob" && (string)x["quest_state"] == "end")
                                              .Select(x => new { objectid = (int)x["objectid"], quantity = (short)x["quantity"] })
                                              .ToDictionary(x => x.objectid, x => x.quantity);
            
            this.ExperienceReward   = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "exp" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "exp" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));
            this.MesoReward         = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "mesos" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "mesos" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));
            this.PetClosenessReward = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_closeness" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_closeness" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));
            this.PetSpeedReward     = new Tuple<bool, bool>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_speed" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0) > 0,
                                                            (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_speed" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0) > 0);
            this.FameReward         = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "fame" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "fame" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));
            this.PetSkillReward     = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_skill" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "pet_skill" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));
            this.BuffReward         = new Tuple<int, int>((int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "buff" && (string)x["quest_state"] == "start")?["rewardid"] ?? 0),
                                                          (int)(rewards.SingleOrDefault(x => (string)x["reward_type"] == "buff" && (string)x["quest_state"] == "end")?["rewardid"] ?? 0));

            this.PreItemRewards     = rewards.Where(x => (string)x["reward_type"] == "item" && (string)x["quest_state"] == "start")
                                             .Select(x => new Tuple<int, short, Gender>((int)x["rewardid"], (short)x["quantity"], (Gender)Enum.Parse(typeof(Gender), ((string)x["gender"]).ToCamel())))
                                             .ToList();
            this.PostItemRewards    = rewards.Where(x => (string)x["reward_type"] == "item" && (string)x["quest_state"] == "end")
                                             .Select(x => new Tuple<int, short, Gender>((int)x["rewardid"], (short)x["quantity"], (Gender)Enum.Parse(typeof(Gender), ((string)x["gender"]).ToCamel())))
                                             .ToList();
            //TODO: Add a constructor on Skill so we can create instances here
            this.PreSkillRewards    = new Dictionary<Skill, Job>();
            this.PostSkillRewards   = new Dictionary<Skill, Job>();
            //this.PreSkillRewards    = rewards.Where(x => (string)x["reward_type"] == "skill" && (string)x["quest_state"] == "start")
            //                                 .Select(x => new { rewardid = (int)x["rewardid"], quantity = (short)x["quantity"], master_level = (short)x["master_level"], job = (short)x["job"] })
            //                                 .ToDictionary(x => new Skill(x.rewardid, x.quantity, x.master_level), x => (Job)x.job);
            //this.PostSkillRewards   = rewards.Where(x => (string)x["reward_type"] == "skill" && (string)x["quest_state"] == "end")
            //                                 .Select(x => new { rewardid = (int)x["rewardid"], quantity = (short)x["quantity"], master_level = (short)x["master_level"], job = (short)x["job"] })
            //                                 .ToDictionary(x => new Skill(x.rewardid, x.quantity, x.master_level), x => (Job)x.job);

            this.ValidJobs = jobs.Select(x => (Job)x["valid_jobid"]).ToList();
        }
    }
}
