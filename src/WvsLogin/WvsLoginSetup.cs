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
            Log.Entitle("Welcome to WsvLogin server Setup");

            Log.Inform("If you do not know what value to put in as your database credentials, leave the field blank to apply default option.");
            Log.SkipLine();
            Log.Inform("Default options are:\n DB Host: localhost;\n DB Schema: login;\n DB Username: root;\n DB Password: ");
            Log.SkipLine();
            Log.Inform("Your advised not to use default username and none password,           not that we care :) ");

            Log.Entitle("Login Database Setup");

            databaseConfiguration:
            Log.Inform("Please enter your login database credentials: ");
            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("DB Host: ", "localhost");
                databaseSchema = Log.Input("DB Schema: ", "login");
                databaseUsername = Log.Input("DB username: ", "root");
                databasePassword = Log.Input("DB password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();
                    Log.SkipLine();
                    Log.Success("Connection to login database was tested and is ready to be populated with data!");
                    Log.SkipLine();

                    if (Log.YesNo("Populate the " + databaseSchema + " as your login server database? ", true))
                    {
                        Log.SkipLine();
                        Log.Inform("Please wait...");
                        PopulateDatabase();
                        Log.Inform("Done populating login database '{0}'!", databaseSchema);
                    }
                }
            }
            catch (MySqlException e)
            {
                Log.SkipLine();
                Log.Error(e);
                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the " + databaseSchema + " as your login server database? ", true))
                {
                    try
                    {
                        Log.SkipLine();
                        Log.Inform("Please wait...");
                        PopulateDatabase();
                        Log.Inform("Database '{0}' created.", databaseSchema);
                    }
                    catch (Exception logindbE)
                    {
                        Log.SkipLine();
                        Log.Error("Error while creating '{0}': ", logindbE, databaseSchema);
                        goto databaseConfiguration;
                    }
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
            Log.Success("Login database configured!");

            Log.Entitle("Login Server Configuration");
            Log.Inform("Again, leave the fields blank to apply default option. \n However keep in mind that the autoregister function is a bit bugged  for now :D");
            Log.SkipLine();

            IPAddress centerIP = Log.Input("Enter the IP of the center server [Default: IPAddress.Loopback]: ", IPAddress.Loopback);
            string securityCode = Log.Input("Assign the security code between servers [Default: ]: ", "");
            bool requireStaffIP = Log.YesNo("Require staff to connect through specific IPs [Default: Yes]? ", true);
            bool autoRegister = Log.YesNo("Allow players to register in-game [Default: Yes]? ", true);
            bool requestPin = Log.YesNo("Require PIN on login [Default: No]? ", false);
            bool requestPic = Log.YesNo("Require PIC upon character selection [Default: No]? ", false);
            int maxCharacters = Log.Input("Maximum characters per account [Default: 3]: ", 3);

            Log.SkipLine();
            Log.Success("Login server configured!");

            Log.SkipLine();
            int worldCount = Log.Input("Enter the number of worlds [Default: 1]: ", 1);

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
                Log.SkipLine();

                worldName[i] = Log.Input("Name [Default: GMSlike]: ", Enum.GetName(typeof(WorldNames), i));
                worldChannels[i] = Log.Input("Channels [Default: 1]: ", 1);
                worldIP[i] = Log.Input("Host IP (external for remote only) [Default: IPAddress.Loopback]: ", IPAddress.Loopback);
                worldEventMessage[i] = Log.Input("Event message [Default: string.Empty]: ", string.Empty);
                worldTickerMessage[i] = Log.Input("Ticker message [Default: string.Empty]: ", string.Empty);

                Log.SkipLine();
                Log.Inform("Please specify the rates: ");
                Log.SkipLine();

                worldExperienceRate[i] = Log.Input("Normal experience [Default: 1]: ", 1);
                worldQuestExperienceRate[i] = Log.Input("Quest experience [Default: 1]: ", 1);
                worldPartyQuestExperienceRate[i] = Log.Input("Party quest experience [Default: 1]: ", 1);
                worldMesoDropRate[i] = Log.Input("Meso drop [Default: 1]: ", 1);
                worldItemDropRate[i] = Log.Input("Item drop [Default: 1]: ", 1);

                Log.SkipLine();
                Log.Inform("Which flag should be shown with this World?\n  None\n  New\n  Hot\n  Event");
                Log.SkipLine();

                worldFlag[i] = WorldFlag.None;

                inputFlag:
                Log.SkipLine();
                try
                {
                    worldFlag[i] = (WorldFlag)Enum.Parse(typeof(WorldFlag), Log.Input("World flag [Default: None]: ", "None"), true);
                }
                catch
                {
                    goto inputFlag;
                }

                Log.SkipLine();
                Log.Success("World '{0}' configured!", worldName[i]);
            }

            Log.Entitle("User Profile Setup");
            Log.Inform("Please choose what detail of debug information you want to display.\n  A. Hide all packets (recommended)[Default]\n  B. Show their names\n  C. Show entire content (expert usage, spam)");
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
            Log.Inform("Writing settings to 'WvsLogin.ini'...");

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

            Log.SkipLine();
            Log.Success("Configuration is done! Login server set up for use successfully.");
        }

        private static void PopulateDatabase()
        {
            Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
                            CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                            USE `{0}`;

					        DROP TABLE IF EXISTS `accounts`;
                            CREATE TABLE `accounts` (
                              `ID` int(10) NOT NULL AUTO_INCREMENT,
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
                              `MaxCharacters` int(11) NOT NULL DEFAULT '3',
                               PRIMARY KEY (`ID`)
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