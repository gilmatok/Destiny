using Destiny.Core.IO;
using Destiny.Core.Network;
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
                            datum["QuestID"] = quest.Value.QuestID;
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
                    this.SetQuestRecord(questID, QuestStatus.NotStarted);
                }
            }
        }

        public void Start(ushort questID, int npcID = 0)
        {
            Start(new Maple.Quest(questID), npcID);
        }

        public void Start(Quest reference, int npcID = 0)
        {
            //Validate pre-start requirements are met
            if (reference.ValidJobs.Count > 0 && !reference.ValidJobs.Contains(this.Parent.Job) && this.Parent.Job != Job.GM && this.Parent.Job != Job.SuperGM)
            {
                this.SendQuestResult(QuestResult.GenericError, reference);
                return;
            }

            foreach (var requiredQuest in reference.PreRequiredQuests)
            {
                switch (requiredQuest.Value)
                {
                    case QuestStatus.Complete:
                        if (!this.Completed.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                    case QuestStatus.InProgress:
                        if (!this.Started.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                    case QuestStatus.NotStarted:
                        if (this.Completed.ContainsKey(requiredQuest.Key) || this.Started.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                }
            }

            if (this.Parent.Meso + reference.MesoReward.Item1 < 0)
            {
                this.SendQuestResult(QuestResult.NotEnoughMesos, reference);
                return;
            }

            foreach (var requiredItem in reference.PreRequiredItems)
            {
                if (!this.Parent.Items.Contains(requiredItem.Key, requiredItem.Value))
                {
                    this.SendQuestResult(QuestResult.GenericError, reference);
                    return;
                }

                //If any of the required items are currently equipped, throw an error
                if (this.Parent.Items.Any(x => x.MapleID == requiredItem.Key && x.IsEquipped))
                {
                    this.SendQuestResult(QuestResult.ItemWornByChar, reference);
                    return;
                }
            }

            var itemRewards = reference.PreItemRewards.Where(x => x.Item3 == this.Parent.Gender || x.Item3 == Gender.Both || this.Parent.Gender == Gender.Both)
                .Select(x => new Item(x.Item1, x.Item2));
            if (!this.Parent.Items.CouldReceive(itemRewards))
            {
                this.SendQuestResult(QuestResult.NoInventorySpace, new Maple.Quest(reference.MapleID));
                return;
            }
            foreach (var item in itemRewards)
            {
                if (item.OnlyOne && this.Parent.Items.Contains(item.MapleID))
                {
                    this.SendQuestResult(QuestResult.OnlyOneOfItemAllowed, reference);
                    return;
                }

                //Finished validation; start handing out rewards

                //Technically, some "rewards" are actually a cost that's part of the requirements, so we have to check if they have a negative quantity
                if (item.Quantity >= 0)
                    this.Parent.Items.Add(item, false);
                else
                    this.Parent.Items.Remove(item.MapleID, Math.Abs(item.Quantity));
            }

            foreach (var skillReward in reference.PreSkillRewards.Where(x => x.Value == this.Parent.Job))
            {
                this.Parent.Skills.Add(skillReward.Key);
            }
            
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

            this.SetQuestRecord(reference.MapleID, QuestStatus.InProgress);
        }

        public void Complete(ushort questID, int selection = 0)
        {
            Complete(new Maple.Quest(questID), selection);
        }

        public void Complete(Quest reference, int selection = 0)
        {
            //Verify that quest requirements have been completed and character has space for rewards
            if (!this.Started.ContainsKey(reference.MapleID))
            {
                this.SendQuestResult(QuestResult.GenericError, reference);
                return;
            }

            foreach (var requiredKill in reference.PostRequiredKills)
            {
                var startedQuestInfo = this.Started[reference.MapleID].FirstOrDefault(x => x.MobID == requiredKill.Key);
                if (startedQuestInfo == null || startedQuestInfo.Killed < requiredKill.Value)
                {
                    this.SendQuestResult(QuestResult.GenericError, reference);
                    return;
                }
            }

            foreach (var requiredQuest in reference.PostRequiredQuests)
            {
                switch (requiredQuest.Value)
                {
                    case QuestStatus.Complete:
                        if (!this.Completed.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                    case QuestStatus.InProgress:
                        if (!this.Started.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                    case QuestStatus.NotStarted:
                        if (this.Completed.ContainsKey(requiredQuest.Key) || this.Started.ContainsKey(requiredQuest.Key))
                        {
                            this.SendQuestResult(QuestResult.GenericError, reference);
                            return;
                        }
                        break;
                }
            }

            if (this.Parent.Meso + reference.MesoReward.Item2 < 0)
            {
                this.SendQuestResult(QuestResult.NotEnoughMesos, reference);
                return;
            }

            foreach (var requiredItem in reference.PostRequiredItems)
            {
                if (!this.Parent.Items.Contains(requiredItem.Key, requiredItem.Value))
                {
                    this.SendQuestResult(QuestResult.GenericError, reference);
                    return;
                }
                
                //If any of the required items are currently equipped, throw an error
                if (this.Parent.Items.Any(x => x.MapleID == requiredItem.Key && x.IsEquipped))
                {
                    this.SendQuestResult(QuestResult.ItemWornByChar, reference);
                    return;
                }
            }

            var itemRewards = reference.PostItemRewards.Where(x => x.Item3 == this.Parent.Gender || x.Item3 == Gender.Both || this.Parent.Gender == Gender.Both)
                .Select(x => new Item(x.Item1, x.Item2));
            if (!this.Parent.Items.CouldReceive(itemRewards))
            {
                this.SendQuestResult(QuestResult.NoInventorySpace, reference);
                return;
            }
            foreach (var item in itemRewards)
            {
                if (item.OnlyOne && this.Parent.Items.Contains(item.MapleID))
                {
                    this.SendQuestResult(QuestResult.OnlyOneOfItemAllowed, reference);
                    return;
                }

                //Finished validation; start handing out rewards
                
                //Technically, some "rewards" are actually a cost that's part of the requirements, so we have to check if they have a negative quantity
                if (item.Quantity >= 0)
                    this.Parent.Items.Add(item, false);
                else
                    this.Parent.Items.Remove(item.MapleID, Math.Abs(item.Quantity));
            }

            foreach (var skillReward in reference.PostSkillRewards.Where(x => x.Value == this.Parent.Job))
            {
                this.Parent.Skills.Add(skillReward.Key);
            }
            
            this.Parent.Experience += reference.ExperienceReward.Item2;
            this.Parent.Meso += reference.MesoReward.Item2;
            this.Parent.Fame += (short)reference.FameReward.Item2;
            //TODO: Pet and buff rewards

            this.Forfeit(reference.MapleID, true); //"Forfeit" the quest to remove it from the quests_started table
            this.Completed.Add(reference.MapleID, new CompletedQuest(reference.MapleID, DateTime.Now));

            this.SetQuestRecord(reference.MapleID, QuestStatus.Complete);
        }

        private void SetQuestRecord(ushort questID, QuestStatus status, string progress = "")
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Message))
            {
                oPacket
                    .WriteByte((byte)MessageType.QuestRecord)
                    .WriteShort((short)questID)
                    .WriteByte((byte)status);

                if (status == QuestStatus.InProgress)
                    oPacket.WriteMapleString(progress);

                if (status == QuestStatus.Complete)
                    oPacket.WriteLong(DateTime.Now.Ticks);

                this.Parent.Client.Send(oPacket);
            }
        }

        public void Handle(InPacket iPacket)
        {
            QuestAction action = (QuestAction)iPacket.ReadByte();
            ushort questID = (ushort)iPacket.ReadShort();

            if (!DataProvider.CachedQuests.Contains(questID))
                return;

            Quest quest = new Maple.Quest(questID);

            int npcId;
            switch (action)
            {
                case QuestAction.RestoreLostItem:
                    int quantity = iPacket.ReadInt();
                    int itemId = iPacket.ReadInt();
                    Item item = new Item(itemId, (short)(quantity - this.Parent.Items.Available(itemId)));
                    this.Parent.Items.Add(item);
                    break;
                case QuestAction.Start:
                    npcId = iPacket.ReadInt();
                    this.Start(quest, npcId);
                    break;
                case QuestAction.Complete:
                    npcId = iPacket.ReadInt();
                    iPacket.ReadInt(); //NOTE: Unknown
                    int selection = iPacket.Remaining >= 4 ? iPacket.ReadInt() : 0;
                    this.Complete(quest, selection);
                    break;
                case QuestAction.Forfeit:
                    this.Forfeit(quest.MapleID);
                    break;
                case QuestAction.ScriptStart:
                    npcId = iPacket.ReadInt();
                    break;
                case QuestAction.ScriptEnd:
                    npcId = iPacket.ReadInt();
                    break;
            }
        }

        private void SendQuestResult(QuestResult operation, Quest quest, int mapleID = 0)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdateQuestInfo))
            {
                oPacket.WriteByte((byte)operation);

                switch (operation)
                {
                    case QuestResult.AddTimeLimit:
                        oPacket
                            .WriteShort(1) //NOTE: Count
                            .WriteShort((short)quest.MapleID)
                            .WriteInt(quest.TimeLimit);
                        break;
                    case QuestResult.RemoveTimeLimit:
                        oPacket
                            .WriteShort(1) //NOTE: Count
                            .WriteShort((short)quest.MapleID);
                        break;
                    case QuestResult.Complete:
                        oPacket
                            .WriteShort((short)quest.MapleID)
                            .WriteInt(Math.Max(mapleID, 0)) //NOTE: NpcID or selection ID
                            .WriteShort((short)quest.NextQuestID);
                        break;
                    case QuestResult.GenericError:
                    case QuestResult.NotEnoughMesos:
                    case QuestResult.ItemWornByChar:
                    case QuestResult.OnlyOneOfItemAllowed:
                        //These don't require any additional bytes
                        break;
                    case QuestResult.NoInventorySpace:
                        oPacket.WriteShort((short)quest.MapleID);
                        break;
                    case QuestResult.Expire:
                        oPacket.WriteShort((short)quest.MapleID);
                        break;
                    case QuestResult.ResetTimeLimit:
                        oPacket.WriteShort((short)quest.MapleID);
                        break;
                }

                this.Parent.Client.Send(oPacket);
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
                        //NOTE: These must be in order since the packet does not include a Mob ID.
                        foreach (var quest in record.Value.OrderBy(x => x.MobID)) 
                        {
                            if (quest.MobID > 0)
                                mobKills.Append(quest.Killed.ToString().PadLeft(3, '\u0030'));
                        }
                        
                        oPacket.WriteMapleString(mobKills.ToString());
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

