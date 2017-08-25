using Destiny.Maple.Characters;
using System;

namespace Destiny.Maple.Commands.Implementation
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
                    string mapName = "";
                    int mapID = int.TryParse(args[0], out mapID) ? mapID : -1;
                    byte portalID = 0;

                    if (args.Length >= 2)
                    {
                        byte.TryParse(args[1], out portalID);
                    }

                    if (mapID == -1)
                    {
                        mapName = string.Join(" ", args);
                        CommandMaps val;
                        Enum.TryParse<CommandMaps>(mapName.ToAlphaNumeric(), true, out val);
                        if (val > 0)
                            mapID = (int)val;
                    }

                    if (mapID > -1)
                    {
                        if (true) // TODO: Check if map exists.
                            caller.ChangeMap(mapID, portalID);
                        else
                            caller.Notify(string.Format("[Command] Invalid map ID {0}.", mapID));
                    }
                    else
                    {
                        caller.Notify(string.Format("[Command] Invalid map name \"{0}\".", mapName));
                    }
                }
            }
        }

        private enum CommandMaps
        {
            MushroomTown = 10000,
            Amherst = 1000000,
            Southperry = 2000000,
            Henesys = 100000000,
            SomeoneElsesHouse = 100000005,
            HenesysMarket = 100000100,
            HenesysPark = 100000200,
            HenesysGamePark = 100000203,
            Ellinia = 101000000,
            MagicLibrary = 101000003,
            ElliniaStation = 101000300,
            Perion = 102000000,
            KerningCity = 103000000,
            SubwayTicketingBooth = 103000100,
            KerningSquare = 103040000,
            LithHarbor = 104000000,
            ThicketAroundtheBeachIII = 104000400,
            ThePigBeach = 104010001,
            Sleepywood = 105040300,
            RegularSauna = 105040401,
            VIPSauna = 105040402,
            AntTunnel = 105050000,
            AntTunnelPark = 105070001,
            TheGraveofMushmom = 105070002,
            OXQuiz = 109020001,
            OlaOla = 109030001,
            MapleStoryPhysicalFitnessTest = 109040000,
            Snowball = 109060000,
            MinigameChallenge = 109070000,
            CoconutHarvest = 109080000,
            FlorinaBeach = 110000000,
            NautilusHarbor = 120000000,
            Ereve = 130000000,
            Rien = 140000000,
            GM = 180000000,
            Blank = 180000001,
            Orbis = 200000000
        }
    }
}
