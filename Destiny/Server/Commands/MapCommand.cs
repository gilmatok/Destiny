using Destiny.Game.Characters;

namespace Destiny.Server.Commands
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
                return "{id}";
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
                int mapID = int.Parse(args[0]);

                if (MasterServer.Instance.Data.Maps.IsValidMap(mapID))
                {
                    caller.ChangeMap(mapID); // TODO: Should we spawn in a randomized spawn point?
                }
                else
                {
                    caller.Notify("[Command] Invalid map.");
                }
            }
        }
    }
}
