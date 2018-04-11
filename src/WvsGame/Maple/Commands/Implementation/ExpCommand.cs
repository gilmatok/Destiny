using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ExpCommand : Command
    {
        public override string Name
        {
            get
            {
                return "exp";
            }
        }

        public override string Parameters
        {
            get
            {
                return "amount";
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
            if (args.Length != 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                if (int.Parse(args[0]) > int.MaxValue || int.Parse(args[0]) < int.MinValue)
                {
                    int expGiven = int.MaxValue;
                    Experience.giveExp(caller, expGiven);
                }
                else
                {
                    int expGiven = int.Parse(args[0]);
                    Experience.giveExp(caller, expGiven);
                }
            }
        }

    }
}