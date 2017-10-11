using Destiny.Maple.Life;
using System.Collections.ObjectModel;
using Destiny.Data;
using System.Collections.Generic;
using Destiny.IO;

namespace Destiny.Maple.Data
{
    public sealed class CachedMobs : KeyedCollection<int, Mob>
    {
        public CachedMobs()
            : base()
        {
            using (Log.Load("Mobs"))
            {
                foreach (Datum datum in new Datums("mob_data").Populate())
                {
                    this.Add(new Mob(datum));
                }

                foreach (Datum datum in new Datums("mob_skills").Populate())
                {
                    if (this.Contains((int)datum["mobid"]))
                    {
                        this[(int)datum["mobid"]].Skills.Add(new MobSkill(datum));
                    }
                }

                foreach (Datum datum in new Datums("skill_mob_data").Populate())
                {
                    foreach (Mob loopMob in this)
                    {
                        foreach (MobSkill loopMobSkill in loopMob.Skills)
                        {
                            if (loopMobSkill.MapleID == (int)(short)datum["skillid"] && loopMobSkill.Level == (byte)(short)datum["skill_level"])
                            {
                                loopMobSkill.Load(datum);
                            }
                        }
                    }
                }

                MobSkill.Summons = new Dictionary<short, List<int>>();

                foreach (Datum mobSummonDatum in new Datums("skill_mob_summons").Populate())
                {
                    if (!MobSkill.Summons.ContainsKey((byte)(short)mobSummonDatum["level"]))
                    {
                        MobSkill.Summons.Add((byte)(short)mobSummonDatum["level"], new List<int>());
                    }

                    MobSkill.Summons[(byte)(short)mobSummonDatum["level"]].Add((int)mobSummonDatum["mobid"]);
                }

                foreach (Datum mobSummonDatum in new Datums("mob_summons").Populate())
                {
                    if (this.Contains((int)mobSummonDatum["mobid"]))
                    {
                        this[(int)mobSummonDatum["mobid"]].DeathSummons.Add((int)mobSummonDatum["summonid"]);
                    }
                }
            }

            using (Log.Load("Loots"))
            {
                foreach (Datum datum in new Datums("drop_data").Populate())
                {
                    int dropperID = (int)datum["dropperid"];

                    if (this.Contains(dropperID))
                    {
                        this[dropperID].Loots.Add(new Loot(datum));
                    }
                }
            }
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.MapleID;
        }
    }
}
