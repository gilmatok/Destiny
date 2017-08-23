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
                caller.Meso += int.Parse(args[0]);
            }
        }
    }
}
