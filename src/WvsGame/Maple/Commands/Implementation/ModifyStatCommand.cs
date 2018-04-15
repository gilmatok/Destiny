using Destiny.Constants;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ModifyStatCommand : Command
    {
        public override string Name
        {
            get
            {
                return "modify";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ stat } [ quantity ]";
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
                short quantity = 0;

                if (args.Length > 1)
                {
                    short.TryParse(args[args.Length - 1], out quantity);
                }

                if (quantity < 1)
                {
                    quantity = 1;
                }

                string stat = args[0];
                CharacterConstants.StatisticType type = CharacterConstants.StatisticType.None;

                switch (stat)
                {
                    case "str":
                        type = CharacterConstants.StatisticType.Strength;
                        break;
                    case "dex":
                        type = CharacterConstants.StatisticType.Dexterity;
                        break;
                    case "int":
                        type = CharacterConstants.StatisticType.Intelligence;
                        break;
                    case "luck":
                        type = CharacterConstants.StatisticType.Luck;
                        break;
                    case "hp":
                        type = CharacterConstants.StatisticType.Health;
                        break;
                    case "maxhp":
                        type = CharacterConstants.StatisticType.MaxHealth;
                        break;
                    case "mp":
                        type = CharacterConstants.StatisticType.Mana;
                        break;
                    case "maxmp":
                        type = CharacterConstants.StatisticType.MaxMana;
                        break;
                    case "ap":
                        type = CharacterConstants.StatisticType.AbilityPoints;
                        break;
                    case "sp":
                        type = CharacterConstants.StatisticType.SkillPoints;
                        break;
                }

                CharacterStats.giveStat(caller, type, quantity);
            }
        }

    }
}