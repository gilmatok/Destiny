using Destiny.Data;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedQuests : KeyedCollection<int, Quest>
    {
        public CachedQuests()
            : base()
        {
            using (Log.Load("Quests"))
            {
                foreach (Datum datum in new Datums("quest_data").Populate())
                {
                    Datums requests = new Datums("quest_requests").Populate("questid = '{0}'", datum["questid"]);
                    Datums rewards = new Datums("quest_rewards").Populate("questid = '{0}'", datum["questid"]);
                    Datums jobs = new Datums("quest_required_jobs").Populate("questid = '{0}'", datum["questid"]);
                    this.Add(new Quest(datum, requests, rewards, jobs));
                }
            }
        }

        protected override int GetKeyForItem(Quest item)
        {
            return item.MapleID;
        }
    }
}
