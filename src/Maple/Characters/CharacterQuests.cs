using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterQuests
    {
        public Character Parent { get; private set; }

        public Dictionary<ushort, List<StartedQuest>> Started { get; private set; }
        public Dictionary<ushort, CompletedQuest> Completed { get; private set; }

        public CharacterQuests(Character parent)
        {
            this.Parent = parent;

            this.Started = new Dictionary<ushort, List<StartedQuest>>();
            this.Completed = new Dictionary<ushort, CompletedQuest>();
        }

        public void Load()
        {
            this.Started = new Datums("quests_started").Populate("CharacterID = '{0}'", this.Parent.ID)
                .GroupBy(x => (ushort)x["QuestID"])
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(row => new StartedQuest(group.Key, (int)row["MobID"], (short)row["Killed"], true)).ToList()
                );

            this.Completed.Clear();
            foreach (Datum datum in new Datums("quests_completed").Populate("CharacterID = '{0}'", this.Parent.ID))
            {
                this.Completed.Add((ushort)datum["QuestID"], new CompletedQuest((ushort)datum["QuestID"], (DateTime)datum["CompletionTime"], true));
            }
        }

        public void Save()
        {
            lock (this.Started)
            {
                lock (this.Completed)
                {
                    foreach (var record in this.Started)
                    {
                        foreach (var quest in record.Value)
                        {
                            Datum datum = new Datum("quests_started");

                            datum["CharacterID"] = this.Parent.ID;
                            datum["QuestID"] = quest.QuestID;
                            datum["MobID"] = quest.MobID;
                            datum["Killed"] = quest.Killed;

                            if (quest.IsAssigned)
                            {
                                datum.Update("CharacterID = '{0}' AND QuestID = '{1}' AND MobID = '{2}'", datum["CharacterID"], datum["QuestID"], datum["MobID"]);
                            }
                            else
                            {
                                datum.Insert();
                                quest.IsAssigned = true;
                            }
                        }
                    }

                    foreach (var quest in this.Completed)
                    {
                        //There is no reason to update a quest already marked as complete; Only insert if it wasn't already in the database
                        if (!quest.Value.IsAssigned)
                        {
                            Datum datum = new Datum("quests_completed");
                            datum["CharacterID"] = this.Parent.ID;
                            datum["CompletionTime"] = quest.Value.CompletionTime;
                            datum.Insert();

                            quest.Value.IsAssigned = true;
                        }
                    }
                }
            }
        }

        public void Delete()
        {
            lock (this.Started)
            {
                lock (this.Completed)
                {
                    foreach (var record in this.Started)
                    {
                        Database.Delete("quests_started", "CharacterID = '{0}' AND QuestID = '{1}'", this.Parent.ID, record.Key);
                    }

                    foreach (var record in this.Completed)
                    {
                        Database.Delete("quests_completed", "CharacterID = '{0}' AND QuestID = '{1}'", this.Parent.ID, record.Key);
                    }
                }
            }
            this.Started.Clear();
            this.Completed.Clear();
        }

        public void Forfeit(ushort questID, bool silent = false)
        {
            Database.Delete("quests_started", "CharacterID = '{0}' AND QuestID = '{1}'", this.Parent.ID, questID);

            if (this.Started.ContainsKey(questID))
            {
                this.Started.Remove(questID);

                if (!silent)
                {
                    //TODO: Send forfeit quest packet
                }
            }
        }

        public bool Start(ushort questID, int? npcID = null)
        {
            var newQuest = new Maple.Quest(questID);
            return Start(newQuest, npcID);
        }

        public bool Start(Quest reference, int? npcID = null)
        {
            //Validate pre-start requirements are met
            if (reference.ValidJobs.Count > 0 && !reference.ValidJobs.Contains(this.Parent.Job) && this.Parent.Job != Job.GM && this.Parent.Job != Job.SuperGM)
                return false;

            foreach (var requiredQuest in reference.PreRequiredQuests)
            {
                switch (requiredQuest.Value)
                {
                    case QuestStatus.Complete:
                        if (!this.Completed.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                    case QuestStatus.InProgress:
                        if (!this.Started.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                    case QuestStatus.NotStarted:
                        if (this.Completed.ContainsKey(requiredQuest.Key) || this.Started.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                }
            }

            foreach (var requiredItem in reference.PreRequiredItems)
            {
                if (!this.Parent.Items.Contains(requiredItem.Key, requiredItem.Value))
                    return false;
            }
            
            var itemRewards = reference.PreItemRewards.Where(x => x.Item3 == this.Parent.Gender || this.Parent.Gender == Gender.Both).Select(x => new Item(x.Item1, x.Item2));
            if (!this.Parent.Items.CouldReceive(itemRewards))
            {
                this.Parent.Items.NotifyFull();
                return false;
            }
            else //Finished validation; start handing out pre-rewards
            {
                //Remove the required items before giving out rewards; previously we only checked if the character had them
                foreach (var requiredItem in reference.PreRequiredItems)
                {
                    this.Parent.Items.Remove(requiredItem.Key, requiredItem.Value);
                }

                foreach (var item in itemRewards)
                {
                    this.Parent.Items.Add(item, false);
                }
            }

            foreach (var skillReward in reference.PreSkillRewards.Where(x => x.Value == this.Parent.Job))
            {
                this.Parent.Skills.Add(skillReward.Key);
            }

            //TODO: Probably should add methods for these rather than adding directly to them
            this.Parent.Experience += reference.ExperienceReward.Item1;
            this.Parent.Meso += reference.MesoReward.Item1;
            this.Parent.Fame += (short)reference.FameReward.Item1;
            //TODO: Pet and buff rewards

            //Create a new entry for each mob, or just 1 if this quest doesn't require mob kills
            List<StartedQuest> entries = new List<StartedQuest>();
            if (reference.PostRequiredKills.Count > 0)
            {
                foreach(var record in reference.PostRequiredKills)
                {
                    entries.Add(new StartedQuest(reference.MapleID, record.Key, 0));
                }
            }
            else
            {
                entries.Add(new StartedQuest(reference.MapleID));
            }

            this.Started.Add(reference.MapleID, entries);
            return true;
        }

        public bool Complete(ushort questID, short? selection = null)
        {
            return Complete(new Maple.Quest(questID), selection);
        }

        public bool Complete(Quest reference, short? selection = null)
        {
            //Verify that quest requirements have been completed and character has space for rewards
            if (this.Started.ContainsKey(reference.MapleID))
                return false;

            foreach (var requiredKill in reference.PostRequiredKills)
            {
                var startedQuestInfo = this.Started[reference.MapleID].FirstOrDefault(x => x.MobID == requiredKill.Key);
                if (startedQuestInfo == null || startedQuestInfo.Killed < requiredKill.Value)
                    return false;
            }

            foreach (var requiredQuest in reference.PostRequiredQuests)
            {
                switch (requiredQuest.Value)
                {
                    case QuestStatus.Complete:
                        if (!this.Completed.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                    case QuestStatus.InProgress:
                        if (!this.Started.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                    case QuestStatus.NotStarted:
                        if (this.Completed.ContainsKey(requiredQuest.Key) || this.Started.ContainsKey(requiredQuest.Key))
                            return false;
                        break;
                }
            }

            foreach (var requiredItem in reference.PostRequiredItems)
            {
                if (!this.Parent.Items.Contains(requiredItem.Key, requiredItem.Value))
                    return false;
            }

            var itemRewards = reference.PostItemRewards.Where(x => x.Item3 == this.Parent.Gender || this.Parent.Gender == Gender.Both).Select(x => new Item(x.Item1, x.Item2));
            if (!this.Parent.Items.CouldReceive(itemRewards))
            {
                this.Parent.Items.NotifyFull();
                return false;
            }
            else //Finished validation; start handing out rewards
            {
                //Remove the required items before giving out rewards; previously we only checked if the character had them
                foreach (var requiredItem in reference.PostRequiredItems)
                {
                    this.Parent.Items.Remove(requiredItem.Key, requiredItem.Value);
                }

                foreach (var item in itemRewards)
                {
                    this.Parent.Items.Add(item, false);
                }
            }

            foreach (var skillReward in reference.PostSkillRewards.Where(x => x.Value == this.Parent.Job))
            {
                this.Parent.Skills.Add(skillReward.Key);
            }

            //TODO: Probably should add methods for these rather than adding directly to them
            this.Parent.Experience += reference.ExperienceReward.Item2;
            this.Parent.Meso += reference.MesoReward.Item2;
            this.Parent.Fame += (short)reference.FameReward.Item2;
            //TODO: Pet and buff rewards

            this.Forfeit(reference.MapleID, true); //"Forfeit" the quest to remove it from the quests_started table
            this.Completed.Add(reference.MapleID, new CompletedQuest(reference.MapleID, DateTime.Now));
            return true;
        }

        public void Handle(InPacket iPacket)
        {
            QuestAction action = (QuestAction)iPacket.ReadByte();
            ushort questId = (ushort)iPacket.ReadShort();

            if (!DataProvider.CachedQuests.Contains(questId))
                return;

            if (action == QuestAction.Forfeit)
            {
                this.Forfeit(questId);
                return;
            }
            else
            {
                int npcId = iPacket.ReadInt();
                if (iPacket.Remaining >= 4)
                {
                    var test = iPacket.ReadInt(); //NOTE: Unknown
                }

                switch (action)
                {
                    case QuestAction.Start:
                        this.Start(questId, npcId);
                        break;
                    case QuestAction.Complete:
                        short? selection = iPacket.Remaining >= 2 ? (short?)iPacket.ReadShort() : null;
                        this.Complete(questId, selection);
                        break;
                    case QuestAction.ScriptStart:
                        break;
                    case QuestAction.ScriptEnd:
                        break;
                }
            }
        }

        public void Encode(OutPacket oPacket)
        {
            lock (this.Started)
            {
                lock (this.Completed)
                {
                    oPacket.WriteShort((short)this.Started.Count);

                    foreach (var record in this.Started)
                    {
                        oPacket.WriteShort((short)record.Key);

                        StringBuilder mobKills = new StringBuilder();
                        foreach (var quest in record.Value)
                        {
                            if (quest.MobID > 0)
                                mobKills.Append(quest.Killed.ToString().PadLeft(3, '\u0030'));
                        }

                        var str = mobKills.ToString();
                        oPacket.WriteString(str.Trim());
                        oPacket.WriteShort();
                    }
                    
                    oPacket.WriteShort((short)this.Completed.Count);

                    foreach (var quest in this.Completed.Values)
                    {
                        oPacket
                            .WriteShort((short)quest.QuestID)
                            .WriteDateTime(quest.CompletionTime);
                    }
                }
            }
        }
    }

    public class StartedQuest
    {
        public ushort QuestID { get; set; }
        public int MobID { get; set; }
        public short Killed { get; set; }
        public bool IsAssigned { get; set; }

        public StartedQuest(ushort questID, int mobID = 0, short killed = 0, bool isAssigned = false)
        {
            this.QuestID = questID;
            this.MobID = mobID;
            this.Killed = killed;
            this.IsAssigned = isAssigned;
        }
    }

    public class CompletedQuest
    {
        public ushort QuestID { get; set; }
        public DateTime CompletionTime { get; set; }
        public bool IsAssigned { get; set; }

        public CompletedQuest(ushort questID, DateTime completionTime, bool isAssigned = false)
        {
            this.QuestID = questID;
            this.CompletionTime = completionTime;
            this.IsAssigned = isAssigned;
        }
    }
}

