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
                StatisticType type = StatisticType.None;

                switch (stat)
                {
                    case "str":
                        type = StatisticType.Strength;
                        break;
                    case "dex":
                        type = StatisticType.Dexterity;
                        break;
                    case "int":
                        type = StatisticType.Intelligence;
                        break;
                    case "luck":
                        type = StatisticType.Luck;
                        break;
                    case "hp":
                        type = StatisticType.Health;
                        break;
                    case "maxhp":
                        type = StatisticType.MaxHealth;
                        break;
                    case "mp":
                        type = StatisticType.Mana;
                        break;
                    case "maxmp":
                        type = StatisticType.MaxMana;
                        break;
                    case "ap":
                        type = StatisticType.AbilityPoints;
                        break;
                    case "sp":
                        type = StatisticType.SkillPoints;
                        break;
                }

                Character.giveStat(caller, type, quantity);
            }
        }

    }
}