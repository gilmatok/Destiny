using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Commands
{
    // TODO: Implement keywords.
    public class MapCommand : Command
    {
        public override string Name => "map";
        public override string Parameters => "{ id | keyword }";
        public override GmLevel RequiredLevel => GmLevel.Intern;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
                this.ShowSyntax(caller);
            else
            {
                int mapID = int.Parse(args[0]);

                //if (MasterServer.Instance.Maps.IsValidMap(mapID))
                //    caller.ChangeMap(mapID); // TODO: Should we spawn in a randomized spawn point?
                //else
                //    caller.Notify("[Command] Invalid map.");
            }
        }
    }
}
