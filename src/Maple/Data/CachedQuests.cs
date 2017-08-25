using Destiny.Core.Data;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Data
{
    public sealed class CachedQuests : KeyedCollection<ushort, Quest>
    {
        public CachedQuests()
            : base()
        {
            using (Log.Load("Quests"))
            {
                foreach (Datum datum in new Datums("quest_data").Populate())
                {
                    this.Add(new Quest(datum));
                }

                foreach (Datum datum in new Datums("quest_requests").Populate())
                {

                }

                foreach (Datum datum in new Datums("quest_rewards").Populate())
                {

                }

                foreach (Datum datum in new Datums("quest_required_jobs").Populate())
                {

                }
            }
        }

        protected override ushort GetKeyForItem(Quest item)
        {
            return item.MapleID;
        }
    }
}
