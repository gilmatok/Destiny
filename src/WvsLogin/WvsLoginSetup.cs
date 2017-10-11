using Destiny.Data;
using Destiny.IO;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;

namespace Destiny
{
    internal static class WvsLoginSetup
    {
        private static string databaseHost = string.Empty;
        private static string databaseSchema = string.Empty;
        private static string databaseUsername = string.Empty;
        private static string databasePassword = string.Empty;

        public static void Run()
        {
            Log.Entitle("WsvLogin Setup");

            Log.Inform("If you do not know a value, leave the field blank to apply default.");

            Log.Entitle("Database Setup");

            databaseConfiguration:

            Log.Inform("Please enter your database credentials: ");

            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("Host: ", "localhost");
                databaseSchema = Log.Input("Database: ", "login");
                databaseUsername = Log.Input("Username: ", "root");
                databasePassword = Log.Input("Password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();

                    if (Log.YesNo("Populate the " + databaseSchema + " database? ", true))
                    {
                        PopulateDatabase();
                    }
                }
            }
            catch (MySqlException e)
            {
                Log.SkipLine();

                Log.Error(e);

                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the " + databaseSchema + " database? ", true))
                {
                    PopulateDatabase();

                    Log.Inform("Database '{0}' created.", databaseSchema);
                }
                else
                {
                    goto databaseConfiguration;
                }
            }
            catch
            {
                Log.SkipLine();

                goto databaseConfiguration;
            }

            Log.SkipLine();

            Log.Success("Database configured!");

            Log.Entitle("Server Configuration");

            IPAddress centerIP = Log.Input("Enter the IP of the center server: ", IPAddress.Loopback);
            string securityCode = Log.Input("Assign the security code between servers: ", "");
            bool requireStaffIP = Log.YesNo("Require staff to connect through specific IPs? ", true);
            bool autoRegister = Log.YesNo("Allow players to register in-game? ", true);
            bool requestPin = Log.YesNo("Require players to enter PIN on login? ", false);
            bool requestPic = Log.YesNo("Require players to enter PIC upon character selection? ", false);
            int maxCharacters = Log.Input("Maximum characters per account: ", 3);

            Log.SkipLine();

            Log.Success("Server configured!");

            int worldCount = Log.Input("Enter the number of worlds: ", 1);

            Log.Entitle("World Configuration");

            string[] worldName = new string[worldCount];
            int[] worldChannels = new int[worldCount];
            IPAddress[] worldIP = new IPAddress[worldCount];
            string[] worldEventMessage = new string[worldCount];
            string[] worldTickerMessage = new string[worldCount];
            int[] worldExperienceRate = new int[worldCount];
            int[] worldQuestExperienceRate = new int[worldCount];
            int[] worldPartyQuestExperienceRate = new int[worldCount];
            int[] worldMesoDropRate = new int[worldCount];
            int[] worldItemDropRate = new int[worldCount];
            WorldFlag[] worldFlag = new WorldFlag[worldCount];

            for (int i = 0; i < worldCount; i++)
            {
                Log.Inform("Please enter the following basic details for World {0}: ", i);

                worldName[i] = Log.Input("Name: ", Enum.GetName(typeof(WorldNames), i));
                worldChannels[i] = Log.Input("Channels: ", 1);
                worldIP[i] = Log.Input("Host IP (external for remote only): ", IPAddress.Loopback);
                worldEventMessage[i] = Log.Input("Event message: ", string.Empty);
                worldTickerMessage[i] = Log.Input("Ticker message: ", string.Empty);

                Log.SkipLine();
                Log.Inform("Please specify the rates: ");

                worldExperienceRate[i] = Log.Input("Normal experience: ", 1);
                worldQuestExperienceRate[i] = Log.Input("Quest experience: ", 1);
                worldPartyQuestExperienceRate[i] = Log.Input("Party quest experience: ", 1);
                worldMesoDropRate[i] = Log.Input("Meso drop: ", 1);
                worldItemDropRate[i] = Log.Input("Item drop: ", 1);

                Log.SkipLine();

                Log.Inform("Which flag should be shown with this World?\n  None\n  New\n  Hot\n  Event");

                worldFlag[i] = WorldFlag.None;

                inputFlag:
                Log.SkipLine();
                try
                {
                    worldFlag[i] = (WorldFlag)Enum.Parse(typeof(WorldFlag), Log.Input("World flag: ", "None"), true);
                }
                catch
                {
                    goto inputFlag;
                }

                Log.SkipLine();

                Log.Success("World '{0}' configured!", worldName[i]);

                Log.SkipLine();
                Log.SkipLine();
            }

            Log.Entitle("User Profile");

            Log.Inform("Please choose what information to display.\n  A. Hide packets (recommended)\n  B. Show names\n  C. Show content");
            Log.SkipLine();

            LogLevel logLevel;

    multipleChoice:
            switch (Log.Input("Please enter yours choice: ", "Hide").ToLower())
            {
                case "a":
                case "hide":
                    logLevel = LogLevel.None;
                    break;

                case "b":
                case "names":
                    logLevel = LogLevel.Name;
                    break;

                case "c":
                case "content":
                    logLevel = LogLevel.Full;
                    break;

                default:
                    goto multipleChoice;
            }

            Log.Entitle("Please wait...");

            Log.Inform("Applying settings to 'WvsLogin.ini'...");

            string lines = string.Format(
                @"[Log]
				Packets={0}
				StackTrace=False
				LoadTime=False
				
				[Server]
				Port=8484
				AutoRegister={1}
				RequestPin={2}
				RequestPic={3}
				MaxCharacters={4}
				RequireStaffIP={5}
				Worlds={6}
				
				[Center]
				IP={7}
				Port=8485
				SecurityCode={8}
				
				[Database]
				Host={9}
				Schema={10}
				Username={11}
				Password={12}",
                logLevel, autoRegister, requestPin, requestPic,
                maxCharacters, requireStaffIP, worldCount, centerIP, securityCode, databaseHost, databaseSchema,
                databaseUsername, databasePassword).Replace("	", "");

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "WvsLogin.ini"))
            {
                file.WriteLine(lines);

                for (int i = 0; i < worldCount; i++)
                {
                    file.WriteLine(string.Format(@"
                    [World{0}]
                    Name={1}
                    Channels={2}
                    HostIP={3}
                    Flag={4}
                    EventMessage={5}
                    TickerMessage={6}
                    ExperienceRate={7}
                    QuestExperienceRate={8}
                    PartyQuestExperienceRate={9}
                    MesoDropRate={10}
                    ItemDropRate={11}",
                    i, worldName[i], worldChannels[i], worldIP[i], worldFlag[i],
                    worldEventMessage[i], worldTickerMessage[i], worldExperienceRate[i], worldQuestExperienceRate[i],
                    worldPartyQuestExperienceRate[i], worldMesoDropRate[i], worldItemDropRate[i]).Replace("  ", ""));
                }
            }

            Log.Success("Configuration done!");
        }

        private static void PopulateDatabase()
        {
            Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
                            CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                            USE `{0}`;

					        DROP TABLE IF EXISTS `accounts`;
                            CREATE TABLE `accounts` (
                              `ID` int(10) NOT NULL,
                              `Username` varchar(12) NOT NULL,
                              `Password` varchar(128) NOT NULL,
                              `Salt` varchar(32) NOT NULL,
                              `EULA` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `Gender` tinyint(3) UNSIGNED NOT NULL DEFAULT '10',
                              `Pin` varchar(64) NOT NULL DEFAULT '',
                              `Pic` varchar(64) NOT NULL DEFAULT '',
                              `IsBanned` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `IsMaster` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `Birthday` date NOT NULL,
                              `Creation` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `MaxCharacters` int(11) NOT NULL DEFAULT '3'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							  DROP TABLE IF EXISTS `banned_ip`;
                            CREATE TABLE `banned_ip` (
                              `Address` varchar(15) NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

							     DROP TABLE IF EXISTS `banned_mac`;
                            CREATE TABLE `banned_mac` (
                              `Address` varchar(17) NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `master_ip`;
                            CREATE TABLE `master_ip` (
                              `IP` varchar(15) NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            ALTER TABLE `accounts`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `username` (`Username`) USING BTREE;

                            ALTER TABLE `banned_ip`
                              ADD PRIMARY KEY (`Address`);

                            ALTER TABLE `banned_mac`
                              ADD PRIMARY KEY (`Address`);

                            ALTER TABLE `master_ip`
                              ADD PRIMARY KEY (`IP`);

							INSERT INTO master_ip VALUES ('127.0.0.1');
						", databaseSchema);
        }
    }
}
