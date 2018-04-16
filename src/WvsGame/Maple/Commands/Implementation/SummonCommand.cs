using System;
using Destiny.Maple.Characters;
using Destiny.Maple.Life;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class SummonCommand : Command
    {
        public override string Name
        {
            get
            {
                return "summon";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ skillID | exact name } [ amount ] ";
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
                int amount = 0;
                bool isAmountSpecified;

                if (args.Length > 1)
                {
                    isAmountSpecified = int.TryParse(args[args.Length - 1], out amount);
                }
                else
                {
                    isAmountSpecified = false;
                }

                if (amount < 1)
                {
                    amount = 1;
                }

                int skillID = -1;

                try
                {
                    skillID = int.Parse(args[0]);
                }
                catch (FormatException)
                {
                    // TODO: Fetch from strings.
                }

                Skill skillToSummonFrom = CharacterSkills.GetNewSkillFromInt(skillID);
                Summon summon = Summon.GetNewSummonFromSkill(skillToSummonFrom);

                // TODO: disperse multiple spawn, they are called with identical position thus overlap
                for (int i = 0; i < amount; i++)
                {
                    caller.Map.Summons.Add(summon);
                }

            }
        }
    }
}