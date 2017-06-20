using Destiny.Utility;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;

namespace Destiny
{
    public static class Setup
    {
        private const string McdbFileName = @"..\..\..\MCDB.sql";

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
                    Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE {0};
							USE {0};
							SET FOREIGN_KEY_CHECKS=0;
                            DROP TABLE IF EXISTS `destiny`.`accounts`;
                            CREATE TABLE  `destiny`.`accounts` (
                              `account_id` int(11) NOT NULL AUTO_INCREMENT,
                              `username` varchar(20) NOT NULL,
                              `password` char(130) NOT NULL DEFAULT '',
                              `salt` blob,
                              PRIMARY KEY (`account_id`),
                              KEY `username` (`username`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
							DROP TABLE IF EXISTS `destiny`.`characters`;
                            CREATE TABLE  `destiny`.`characters` (
                              `character_id` int(11) NOT NULL AUTO_INCREMENT,
                              `name` varchar(12) NOT NULL,
                              `account_id` int(11) NOT NULL,
                              `world_id` tinyint(3) unsigned NOT NULL,
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
                              `mesos` int(11) NOT NULL DEFAULT '0',
                              `equipment_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `usable_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `setup_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `etcetera_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
                              `cash_slots` tinyint(3) unsigned NOT NULL DEFAULT '48',
                              `buddylist_size` int(3) NOT NULL DEFAULT '20',
                              `overall_cpos` int(11) DEFAULT NULL,
                              `overall_opos` int(11) DEFAULT NULL,
                              `world_cpos` int(11) DEFAULT NULL,
                              `world_opos` int(11) DEFAULT NULL,
                              `job_cpos` int(11) DEFAULT NULL,
                              `job_opos` int(11) DEFAULT NULL,
                              `fame_cpos` int(11) DEFAULT NULL,
                              `fame_opos` int(11) DEFAULT NULL,
                              `book_cover` int(11) DEFAULT NULL,
                              PRIMARY KEY (`character_id`),
                              KEY `account_id` (`account_id`),
                              KEY `world_id` (`world_id`),
                              KEY `name` (`name`),
                              CONSTRAINT `characters_ibfk_1` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
                            DROP TABLE IF EXISTS `destiny`.`items`;
                            CREATE TABLE  `destiny`.`items` (
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
                              `pet_id` bigint(23) unsigned DEFAULT NULL,
                              `name` varchar(12) DEFAULT NULL,
                              `expiration` datetime DEFAULT NULL,
                              PRIMARY KEY (`character_id`,`inventory`,`slot`),
                              CONSTRAINT `items_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`character_id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
						", databaseSchema);

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

                        Log.Inform("Database 'mcdb' created.", databaseSchema);
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
