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
                return "{ amount }";
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
				if (int.TryParse(args[0], out int mesoGiven))
				{
					Meso.giveMesos(caller, mesoGiven);
				}
				else
				{
					caller.Notify("[Command] Invalid amount provided.");
				}
            }
        }
    }
}
