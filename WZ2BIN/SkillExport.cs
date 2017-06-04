using Destiny;
using Destiny.Server.Data;
using reWZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WZ2BIN
{
    internal static class SkillExport
    {
        public static void Export(string inputPath, string outputPath)
        {
            List<SkillData> skills = new List<SkillData>();
            List<AbilityData> abilities = new List<AbilityData>();

            using (WZFile file = new WZFile(Path.Combine(inputPath, "Skill.wz"), WZVariant.GMS, true, WZReadSelection.None))
            {
                foreach (var jobNode in file.MainDirectory)
                {
                    if (jobNode.Name == "MCGuardian.img" ||
                        jobNode.Name == "MCSkill.img" ||
                        jobNode.Name == "BFSkill.img" ||
                        jobNode.Name == "ItemSkill.img") // TODO: Deal with these later.
                    {
                        continue;
                    }

                    if (jobNode.Name == "MobSkill.img")
                    {
                        foreach (var skillNode in jobNode)
                        {
                            foreach (var levelNode in skillNode["level"])
                            {
                                AbilityData ability = new AbilityData();

                                ability.Identifier = (byte)skillNode.GetID();
                                ability.Level = byte.Parse(levelNode.Name);
                                ability.Duration = 0;
                                ability.MPCost = (byte)levelNode.GetInt("mpCon");
                                ability.Parameter1 = levelNode.GetInt("x");
                                ability.Parameter2 = levelNode.GetInt("y");
                                ability.Chance = 0;
                                ability.TargetCount = 0;
                                ability.Cooldown = (ushort)levelNode.GetInt("time");
                                ability.LeftTopX = 0; // lt
                                ability.LeftTopY = 0; // lt
                                ability.RightBottomX = 0; // rb
                                ability.RightBottomY = 0; // rb
                                ability.HPLimitPercent = 0;
                                ability.SummonLimit = 0;
                                ability.SummonEffect = 0;

                                ability.SummonIdentifiers = new List<int>();

                                if (levelNode.HasChild("summon"))
                                {

                                }

                                abilities.Add(ability);
                            }
                        }
                    }
                    else
                    {
                        foreach (var skillNode in jobNode["skill"])
                        {
                            foreach (var levelNode in skillNode["level"])
                            {
                                SkillData skill = new SkillData();

                                skill.MapleID = skillNode.GetID();
                                skill.Level = byte.Parse(levelNode.Name);
                                skill.MobCount = (byte)levelNode.GetInt("mobCount");
                                skill.HitCount = (byte)levelNode.GetInt("attackCount");
                                skill.Range = (ushort)levelNode.GetInt("range");
                                skill.Duration = 0;
                                skill.MPCost = (ushort)skillNode.GetInt("mpCon");
                                skill.HPCost = (byte)skillNode.GetInt("hpCon");
                                skill.Damage = 0;
                                skill.FixedDamage = 0; // TODO: The 'damage' property is uncastable for some reason. Check why.
                                skill.CriticalDamage = 0;
                                skill.Mastery = 0;
                                skill.OptionalItemCost = 0;
                                skill.ItemCost = levelNode.GetInt("itemCon");
                                skill.ItemCount = (byte)levelNode.GetInt("itemConNo");
                                skill.BulletCost = (byte)levelNode.GetInt("bulletConsume");
                                skill.MoneyCost = (ushort)levelNode.GetInt("moneyCon");
                                skill.Parameter1 = levelNode.GetInt("x");
                                skill.Parameter2 = levelNode.GetInt("y");
                                skill.Speed = (byte)levelNode.GetInt("speed");
                                skill.Jump = (byte)levelNode.GetInt("jump");
                                skill.Strength = 0;
                                skill.WeaponAttack = 0;
                                skill.MagicAttack = 0;
                                skill.WeaponDefense = 0;
                                skill.MagicDefense = 0;
                                skill.Accuracy = 0;
                                skill.Avoidance = 0;
                                skill.HP = (byte)levelNode.GetInt("hp");
                                skill.MP = (byte)levelNode.GetInt("mp");
                                skill.Prop = (byte)levelNode.GetInt("prop");
                                skill.Morph = 0;
                                skill.LeftTopX = 0; // lt
                                skill.LeftTopY = 0; // lt
                                skill.RightBottomX = 0; // rb
                                skill.RightBottomY = 0; // rb
                                skill.Cooldown = (ushort)levelNode.GetInt("time");

                                skills.Add(skill);
                            }
                        }
                    }
                }

                skills = skills.OrderBy(m => m.MapleID).ToList();
                abilities = abilities.OrderBy(a => a.Identifier).ToList();

                using (FileStream stream = File.Create(Path.Combine(outputPath, "Skills.bin")))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(skills.Count);

                        foreach (SkillData skill in skills)
                        {
                            skill.Save(writer);
                        }
                    }
                }

                Logger.Write(LogLevel.Info, "Exported {0} skills.", skills.Count);

                skills.Clear();

                using (FileStream stream = File.Create(Path.Combine(outputPath, "Abilities.bin")))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(abilities.Count);

                        foreach (AbilityData ability in abilities)
                        {
                            ability.Save(writer);
                        }
                    }
                }

                Logger.Write(LogLevel.Info, "Exported {0} abilities.", abilities.Count);

                abilities.Clear();
            }
        }
    }
}