using Destiny.Utility;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;

namespace Destiny
{
    public static class Setup
    {
        private const string DbFileName = @"..\..\Destiny.sql";
        private const string McdbFileName = @"..\..\MCDB.sql";

        public static void Run()
        {
            Log.Entitle("Destiny Setup");

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
                databaseSchema = Log.Input("Schema: ", "destiny");
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

                        Database.ExecuteFile(databaseHost, databaseUsername, databasePassword, Application.ExecutablePath + Setup.DbFileName);

                        Log.Inform("Database '{0}' created.", databaseSchema);
                    }
                    catch (Exception mcdbE)
                    {
                        Log.Error("Error while creating '{0}': ", mcdbE, databaseSchema);

                        goto mcdbConfiguration;
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

        mcdbConfiguration:
            Log.Inform("The setup will now check for a MapleStory database.");

            try
            {
                using (Database.TemporaryConnection(databaseHost, "mcdb", databaseUsername, databasePassword))
                {
                    Database.Test();
                }
            }
            catch (MySqlException e)
            {
                Log.Error(e);

                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo("Create and populate the MCDB database? ", true))
                {
                    try
                    {
                        Log.Inform("Please wait...");

                        Database.ExecuteFile(databaseHost, databaseUsername, databasePassword, Application.ExecutablePath + Setup.McdbFileName);

                        Log.Inform("Database 'mcdb' created.");
                    }
                    catch (Exception mcdbE)
                    {
                        Log.Error("Error while creating 'mcdb': ", mcdbE);

                        goto mcdbConfiguration;
                    }
                }
                else
                {
                    Log.SkipLine();

                    goto mcdbConfiguration;
                }
            }

            Log.SkipLine();

            Log.Success("Database configured!");

            Log.Entitle("Server Configuration");

            string serverName = Log.Input("Enter the server's name: ", "Destiny");
            int channels = Log.Input("Enter the number of channels: ", 2);
            IPAddress externalIP = Log.Input("Enter the public server IP: ", IPAddress.Loopback);

            Log.SkipLine();

            Log.Success("Server configured!");

            Log.Entitle("Please wait...");

            Log.Inform("Applying settings to 'Configuration.ini'...");

            string lines = string.Format(
                @"[Log]
				StackTrace=False
				LoadTime=False
				
				[Login]
				Port=8484
				
				[Server]
				Name={0}
				Channels={1}
				ExternalIP={2}
				AutoRestartTime=30
				
				[Database]
				Host={3}
				Schema={4}
				Username={5}
				Password={6}",
                serverName, channels, externalIP,
                databaseHost, databaseSchema,
                databaseUsername, databasePassword).Replace("	", "");

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "Configuration.ini"))
            {
                file.WriteLine(lines);
            }

            Log.Success("Configuration done!");
        }
    }
}
