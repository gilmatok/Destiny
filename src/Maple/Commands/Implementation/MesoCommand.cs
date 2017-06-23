using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
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

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.Gm;
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
