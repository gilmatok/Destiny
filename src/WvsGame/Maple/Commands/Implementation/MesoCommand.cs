using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class MesoCommand : Command
    {
        public override string Name
        {
            get
            {
                return "meso";
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
                    int mesoGiven = int.MaxValue;
                    Meso.giveMesos(caller, mesoGiven);
                }
                else
                {
                    int mesoGiven = int.Parse(args[0]);
                    Meso.giveMesos(caller, mesoGiven);
                }
            }
        }
    }
}
