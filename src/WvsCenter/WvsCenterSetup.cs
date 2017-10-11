using Destiny.Data;
using Destiny.IO;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Destiny
{
    public static class WvsCenterSetup
    {
        public static void Run()
        {
            Log.Entitle("WvsCenter Setup");

            Log.Inform("If you do not know a value, leave the field blank to apply default.");

            Log.Entitle("Database Setup");

            string databaseHost = string.Empty;
            string databaseSchema = string.Empty;
            string databaseUsername = string.Empty;
            string databasePassword = string.Empty;

            databaseConfiguration:

            Log.Inform("Please enter your database credentials: ");

            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("Host: ", "localhost");
                databaseSchema = Log.Input("Schema: ", "center");
                databaseUsername = Log.Input("Username: ", "root");
                databasePassword = Log.Input("Password: ", "");

                using (Database.TemporaryConnection(databaseHost, databaseSchema, databaseUsername, databasePassword))
                {
                    Database.Test();
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
                        Log.Inform("Please wait...");

                        Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                            USE `{0}`;
                            ", databaseSchema);

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
            
            Log.Success("Database configured!");

            Log.Entitle("Server Configuration");

            string securityCode = Log.Input("Assign the security code between servers: ", "");
            int autoRestartTime = Log.Input("Automatic restart time (leave blank for none): ", 15);

            Log.SkipLine();

            Log.Success("Server configured!");

            Log.Entitle("User Profile");

            Log.Inform("Please choose what information to display.\n  A. Hide packets (recommended)\n  B. Show names\n  C. Show content");
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

            Log.Inform("Applying settings to 'WvsCenter.ini'...");

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

            Log.Success("Configuration done!");
        }
    }
}