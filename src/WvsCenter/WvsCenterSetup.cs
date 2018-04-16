using Destiny.Data;
using Destiny.IO;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Destiny
{
    public static class WvsCenterSetup
    {
        private static string databaseHost = string.Empty;
        private static string databaseSchema = string.Empty;
        private static string databaseUsername = string.Empty;
        private static string databasePassword = string.Empty;

        public static void Run()
        {
            Log.Entitle("Welcome to WvsCenter Server Setup");

            Log.Inform("If you do not know what value to put in as your database credentials, leave the fields blank to apply default option.");
            Log.SkipLine();
            Log.Inform("Default options are:\n DB Host: localhost;\n DB Schema: center;\n DB Username: root;\n DB Password: ;");
            Log.SkipLine();
            Log.Inform("Your advised not to use default username and none password,           not that we care :) ");

            Log.Entitle("Center Database Setup");

            databaseConfiguration:
            Log.Inform("Please enter your center database credentials: ");
            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("DB Host: ", "localhost");
                databaseSchema = Log.Input("DB Schema: ", "center");
                databaseUsername = Log.Input("DB Username: ", "root");
                databasePassword = Log.Input("DB Password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();
                    Log.SkipLine();
                    Log.Success("Connection to center database was tested and is ready to be used!");
                }
            }
            catch (MySqlException e)
            {
                Log.SkipLine();
                Log.Error(e);
                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the " + databaseSchema + " database? ", true))
                {
                    try
                    {
                        Log.SkipLine();
                        Log.Inform("Please wait...");
                        PopulateCenterDatabase();
                        Log.Inform("Database '{0}' created.", databaseSchema);
                    }
                    catch (Exception mcdbE)
                    {
                        Log.Error("Error while creating '{0}': ", mcdbE, databaseSchema);
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
            Log.Success("Center Database configured!");

            Log.Entitle("Center Server Configuration");
            Log.Inform("You can leave the fields blank to apply default option.");
            Log.SkipLine();

            string securityCode = Log.Input("Assign the security code between servers [Default: ]: ", "");
            int autoRestartTime = Log.Input("Automatic restart time in seconds [Default: 15]: ", 15);

            Log.SkipLine();
            Log.Success("Center Server configured!");

            Log.Entitle("User Profile Setup");
            Log.Inform("Please choose what detail of debug information you want to display.\n  A. Hide packets (recommended)[Default]\n  B. Show names\n  C. Show hex content (expert usage, spam)");
            Log.SkipLine();

            LogLevel logLevel;

            multipleChoice:
            switch (Log.Input("Please enter your choice: ", "Hide").ToLower())
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
            Log.Inform("Writing settings to 'WvsCenter.ini'...");

            string lines = string.Format(
                @"[Log]
                Packets={0}
                StackTrace=False
                LoadTime=False
                JumpLists=3
    
                [Server]
                Port=8485
                SecurityCode={1}
    
                [Database]
                Host={2}
                Schema={3}
                Username={4}
                Password={5}",
                logLevel, securityCode, databaseHost,
                databaseSchema, databaseUsername, databasePassword).Replace("  ", "");

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "WvsCenter.ini"))
            {
                file.WriteLine(lines);
            }

            Log.SkipLine();
            Log.Success("Configuration is done! Center server set up for use successfully.");
        }

        private static void PopulateCenterDatabase()
        {
            Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                            USE `{0}`;
                            ", databaseSchema);
        }

    }
}