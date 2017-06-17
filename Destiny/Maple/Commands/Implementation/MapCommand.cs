using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    // TODO: Implement keywords.
    public class MapCommand : Command
    {
        public override string Name
        {
            get
            {
                return "map";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ id | keyword } [ portal ]";
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.Intern;
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

            }
        }
    }
}
