using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Network;
using System;

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

        public override bool IsRestricted
        {
            get
            {
                return true;
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
                    int mapID = int.TryParse(args[0], out mapID) ? mapID : -1;
                    byte portalID = 0;

                    if(args.Length >= 2)
                    {
                        byte.TryParse(args[1], out portalID);
                    }

                    if (mapID == -1)
                    {
                        CommandMaps val;
                        Enum.TryParse<CommandMaps>(args[0].Replace(" ", ""), true, out val);
                        if (val > 0)
                            mapID = (int)val;
                    }
                    
                    if (mapID > -1)
                    {
                        if(DataProvider.Maps.Contains(mapID))
                            caller.ChangeMap(mapID, portalID);
                        else
                            caller.Notify(string.Format("[Command] Invalid map ID {0}.", mapID));
                    }
                    else
                    {
                        caller.Notify(string.Format("[Command] Invalid map '{0}'.", args[0]));
                    }
                }
            }
        }

        private enum CommandMaps
        {
            //TODO: Add more of these - Proof of concept only
            Amherst = 1000000,
            Southperry = 2000000,
            Sleepywood = 105040300
        }
    }
}
