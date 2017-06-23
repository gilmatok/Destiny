using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
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
                return "{ { id | keyword | exact name } [portal] | -current }";
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
            if (args.Length == 0)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                if (args.Length == 1 && args[0] == "-current")
                {
                    caller.Notify("[Command] Current map: " + caller.Map.MapleID);
                    caller.Notify("   -X: " + caller.Position.X);
                    caller.Notify("   -Y: " + caller.Position.Y);
                }
                else
                {

                }
            }
        }
    }
}
