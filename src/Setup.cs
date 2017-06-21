using Destiny.Utility;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;

namespace Destiny
{
    public static class Setup
    {
        private const string DbFileName = @"..\..\sql\Destiny.sql";
        private const string McdbFileName = @"..\..\sql\MCDB.sql";

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

                        Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE {0};
							USE {0};

							SET FOREIGN_KEY_CHECKS=0;

                            CREATE TABLE `accounts` (
                              `account_id` int(10) NOT NULL AUTO_INCREMENT,
                              `username` varchar(12) NOT NULL,
                              `password` varchar(128) NOT NULL,
                              `salt` varchar(32) NOT NULL,
                              `gm_level` tinyint(3) unsigned NOT NULL,
                              PRIMARY KEY (`account_id`),
                              KEY `username` (`username`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

                            CREATE TABLE `characters` (
                              `character_id` int(11) NOT NULL AUTO_INCREMENT,
                              `account_id` int(11) NOT NULL,
                              `name` varchar(13) NOT NULL,
                              `gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
                              `skin` tinyint(4) unsigned NOT NULL DEFAULT '0',
                              `face` int(11) NOT NULL,
                              `hair` int(11) NOT NULL,
                              `level` tinyint(3) unsigned NOT NULL DEFAULT '1',
                              `job` smallint(6) NOT NULL DEFAULT '0',
                              `strength` smallint(6) NOT NULL DEFAULT '12',
                              `dexterity` smallint(6) NOT NULL DEFAULT '5',
                              `intelligence` smallint(6) NOT NULL DEFAULT '4',
                              `luck` smallint(6) NOT NULL DEFAULT '4',
                              `health` smallint(6) NOT NULL DEFAULT '50',
                              `max_health` smallint(6) NOT NULL DEFAULT '50',
                              `mana` smallint(6) NOT NULL DEFAULT '5',
                              `max_mana` smallint(6) NOT NULL DEFAULT '5',
                              `ability_points` smallint(6) NOT NULL DEFAULT '0',
                              `skill_points` smallint(6) NOT NULL DEFAULT '0',
                              `experience` int(11) NOT NULL DEFAULT '0',
                              `fame` smallint(6) NOT NULL DEFAULT '0',
                              `map` int(11) NOT NULL DEFAULT '0',
                              `map_spawn` tinyint(3) unsigned NOT NULL DEFAULT '0',
                              `meso` int(11) NOT NULL DEFAULT '0',
                              `equipment_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `usable_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `setup_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `etcetera_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `cash_slots` tinyint(3) unsigned NOT NULL DEFAULT '48',
                              `buddylist_size` int(3) NOT NULL DEFAULT '20',
                              PRIMARY KEY (`character_id`),
                              KEY `account_id` (`account_id`),
                              KEY `name` (`name`),
                              CONSTRAINT `characters_ibfk_1` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

                            CREATE TABLE `items` (
                              `character_id` int(11) NOT NULL DEFAULT '0',
                              `inventory` tinyint(3) unsigned NOT NULL DEFAULT '0',
                              `slot` smallint(6) NOT NULL DEFAULT '0',
                              `item_identifier` int(11) NOT NULL DEFAULT '0',
                              `quantity` smallint(6) NOT NULL DEFAULT '1',
                              `slots` tinyint(4) unsigned DEFAULT NULL,
                              `scrolls` tinyint(4) unsigned DEFAULT NULL,
                              `strength` smallint(6) DEFAULT NULL,
                              `dexterity` smallint(6) DEFAULT NULL,
                              `intelligence` smallint(6) DEFAULT NULL,
                              `luck` smallint(6) DEFAULT NULL,
                              `health` smallint(6) DEFAULT NULL,
                              `mana` smallint(6) DEFAULT NULL,
                              `weapon_attack` smallint(6) DEFAULT NULL,
                              `magic_attack` smallint(6) DEFAULT NULL,
                              `weapon_defense` smallint(6) DEFAULT NULL,
                              `magic_defense` smallint(6) DEFAULT NULL,
                              `accuracy` smallint(6) DEFAULT NULL,
                              `avoidability` smallint(6) DEFAULT NULL,
                              `hands` smallint(6) DEFAULT NULL,
                              `speed` smallint(6) DEFAULT NULL,
                              `jump` smallint(6) DEFAULT NULL,
                              `flags` tinyint(3) DEFAULT NULL,
                              `hammers` tinyint(3) DEFAULT NULL,
                              `name` varchar(12) DEFAULT NULL,
                              `expiration` datetime DEFAULT NULL,
                              PRIMARY KEY (`character_id`,`inventory`,`slot`),
                              CONSTRAINT `items_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`character_id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
                            ", databaseSchema);

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
