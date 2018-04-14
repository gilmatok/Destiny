using System;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using System.Linq;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class LearnSkillCommand : Command
    {
        public override string Name
        {
            get
            {
                return "learn";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ skillID } [ skillLvl ]";
            }
        }

        public override bool IsRestricted
        {
            get
            {
                return true;
            }
        }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                int skillID = int.Parse(args[0]);
                int skillLVL = int.Parse(args[1]);
             
                if (DataProvider.Skills.ContainsKey(skillID))
                {
                    if (caller.Skills.Contains(skillID))
                    {
                        Skill skillToModify = caller.Skills[skillID];

                        skillToModify.CurrentLevel = (byte)skillLVL;
                        skillToModify.Update();
                    }
                    else if(!caller.Skills.Contains(skillID))
                    {
                        Skill skillToAdd = new Skill(skillID);

                        skillToAdd.CurrentLevel = (byte) skillLVL;
                        skillToAdd.MapleID = skillID;
                        skillToAdd.Update();
                    }
                }
                else
                {
                    caller.Notify("[Command] Invalid skill ID.");
                }

            }
        }

    }
}