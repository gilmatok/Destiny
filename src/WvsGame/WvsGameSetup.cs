using Destiny.Data;
using Destiny.IO;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;

namespace Destiny
{
    public static class WvsGameSetup
    {
        private const string McdbFileName = @"..\..\sql\MCDB.sql";

        private static string databaseHost = string.Empty;
        private static string databaseSchema = string.Empty;
        private static string databaseSchemaMCDB = string.Empty;
        private static string databaseUsername = string.Empty;
        private static string databasePassword = string.Empty;

        public static void Run()
        {
            Log.Entitle("WvsGame Setup");

            Log.Inform("If you do not know a value, leave the field blank to apply default.");

            Log.Entitle("Database Setup");

            databaseConfiguration:

            Log.Inform("Please enter your database credentials: ");

            Log.SkipLine();

            try
            {
                databaseHost = Log.Input("Host: ", "localhost");
                databaseSchema = Log.Input("Schema: ", "game");
                databaseSchemaMCDB = Log.Input("MCDB Schema: ", "mcdb");
                databaseUsername = Log.Input("Username (that can access both databases): ", "root");
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
                    try
                    {
                        Log.Inform("Please wait...");

                        PopulateDatabase();

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
                using (Database.TemporaryConnection(databaseHost, databaseSchemaMCDB, databaseUsername, databasePassword, true))
                {
                    Database.Test();

                    if (Log.YesNo($"Populate the '{databaseSchemaMCDB}' database? ", true))
                    {
                        Database.ExecuteFile(databaseHost, databaseUsername, databasePassword, Application.ExecutablePath + McdbFileName);
                    }
                }
            }
            catch (MySqlException e)
            {
                Log.Error(e);

                Log.SkipLine();

                if (e.Message.Contains("Unknown database") && Log.YesNo($"Create and populate the '{databaseSchemaMCDB}' database? ", true))
                {
                    try
                    {
                        Log.Inform("Please wait...");

                        Database.ExecuteFile(databaseHost, databaseUsername, databasePassword, Application.ExecutablePath + McdbFileName);

                        Log.Inform($"Database '{databaseSchemaMCDB}' created.");
                    }
                    catch (Exception mcdbE)
                    {
                        Log.Error($"Error while creating '{databaseSchemaMCDB}': ", mcdbE);

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

            IPAddress centerIP = Log.Input("Enter the IP of the center server: ", IPAddress.Loopback);
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

            Log.Inform("Applying settings to 'WvsGame.ini'...");

            string lines = string.Format(
                @"[Log]
                Packets={0}
                StackTrace=False
                LoadTime=False
                JumpLists=3
    
                [Server]
                AutoRestartTime={1}

                [Center]
                IP={2}
                Port=8485
                SecurityCode={3}
    
                [Database]
                Host={4}
                Schema={5}
                SchemaMCDB={6}
                Username={7}
                Password={8}",
                logLevel, autoRestartTime, centerIP, securityCode, databaseHost,
                databaseSchema, databaseSchemaMCDB, databaseUsername, databasePassword).Replace("  ", "");

            using (StreamWriter file = new StreamWriter(Application.ExecutablePath + "WvsGame.ini"))
            {
                file.WriteLine(lines);
            }

            Log.Success("Configuration done!");
        }

        private static void PopulateDatabase()
        {
            Database.ExecuteScript(databaseHost, databaseUsername, databasePassword, @"
							CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                            USE `{0}`;
                          
                            DROP TABLE IF EXISTS `buffs`;
                            CREATE TABLE `buffs` (
                              `ID` int(11) NOT NULL,
                              `CharacterID` int(11) NOT NULL,
                              `Type` tinyint(3) UNSIGNED NOT NULL,
                              `MapleID` int(11) NOT NULL,
                              `SkillLevel` int(11) NOT NULL,
                              `Value` int(11) NOT NULL,
                              `End` datetime NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `characters`;
                            CREATE TABLE `characters` (
                              `ID` int(11) NOT NULL,
                              `AccountID` int(11) NOT NULL,
                              `WorldID` tinyint(3) UNSIGNED NOT NULL,
                              `Name` varchar(13) NOT NULL,
                              `Level` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
                              `Experience` int(11) NOT NULL DEFAULT '0',
                              `Job` smallint(6) NOT NULL DEFAULT '0',
                              `Strength` smallint(6) NOT NULL,
                              `Dexterity` smallint(6) NOT NULL,
                              `Luck` smallint(6) NOT NULL,
                              `Intelligence` smallint(6) NOT NULL,
                              `Health` smallint(6) NOT NULL DEFAULT '50',
                              `MaxHealth` smallint(6) NOT NULL DEFAULT '50',
                              `Mana` smallint(6) NOT NULL DEFAULT '5',
                              `MaxMana` smallint(6) NOT NULL DEFAULT '5',
                              `Meso` int(10) NOT NULL DEFAULT '0',
                              `Fame` smallint(6) NOT NULL DEFAULT '0',
                              `Gender` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Hair` int(11) NOT NULL,
                              `Skin` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Face` int(11) NOT NULL,
                              `AbilityPoints` smallint(6) NOT NULL DEFAULT '0',
                              `SkillPoints` smallint(6) NOT NULL DEFAULT '0',
                              `MapID` int(11) NOT NULL DEFAULT '0',
                              `SpawnPoint` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `MaxBuddies` tinyint(3) UNSIGNED NOT NULL DEFAULT '20',
                              `EquipmentSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '24',
                              `UsableSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '24',
                              `SetupSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '24',
                              `EtceteraSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '24',
                              `CashSlots` tinyint(3) UNSIGNED NOT NULL DEFAULT '48',
                              `PartyID` int(11) DEFAULT NULL,
                              `GuildID` int(11) DEFAULT NULL,
                              `GuildRank` int(11) DEFAULT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `guilds`;
                            CREATE TABLE `guilds` (
                              `ID` int(11) NOT NULL,
                              `WorldID` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Name` varchar(13) NOT NULL DEFAULT '',
                              `Notice` varchar(100) NOT NULL DEFAULT '',
                              `Rank1` varchar(13) NOT NULL DEFAULT '',
                              `Rank2` varchar(13) NOT NULL DEFAULT '',
                              `Rank3` varchar(13) NOT NULL DEFAULT '',
                              `Rank4` varchar(13) NOT NULL DEFAULT '',
                              `Rank5` varchar(13) NOT NULL DEFAULT '',
                              `Logo` smallint(6) NOT NULL DEFAULT '0',
                              `LogoColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Background` smallint(6) NOT NULL DEFAULT '0',
                              `BackgroundColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `items`;
                            CREATE TABLE `items` (
                              `ID` int(11) NOT NULL,
                              `AccountID` int(10) NOT NULL,
                              `CharacterID` int(10) NOT NULL,
                              `MapleID` int(11) NOT NULL,
                              `Slot` smallint(6) NOT NULL DEFAULT '0',
                              `Creator` varchar(13) DEFAULT NULL,
                              `UpgradesAvailable` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `UpgradesApplied` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Strength` smallint(6) NOT NULL DEFAULT '0',
                              `Dexterity` smallint(6) NOT NULL DEFAULT '0',
                              `Intelligence` smallint(6) NOT NULL DEFAULT '0',
                              `Luck` smallint(6) NOT NULL DEFAULT '0',
                              `Health` smallint(6) NOT NULL DEFAULT '0',
                              `Mana` smallint(6) NOT NULL DEFAULT '0',
                              `WeaponAttack` smallint(6) NOT NULL DEFAULT '0',
                              `MagicAttack` smallint(6) NOT NULL DEFAULT '0',
                              `WeaponDefense` smallint(6) NOT NULL DEFAULT '0',
                              `MagicDefense` smallint(6) NOT NULL DEFAULT '0',
                              `Accuracy` smallint(6) NOT NULL DEFAULT '0',
                              `Avoidability` smallint(6) NOT NULL DEFAULT '0',
                              `Agility` smallint(6) NOT NULL DEFAULT '0',
                              `Speed` smallint(6) NOT NULL DEFAULT '0',
                              `Jump` smallint(6) NOT NULL DEFAULT '0',
                              `IsScissored` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `PreventsSlipping` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `PreventsColdness` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `IsStored` tinyint(1) UNSIGNED NOT NULL DEFAULT '0',
                              `Quantity` smallint(6) NOT NULL DEFAULT '1',
                              `Expiration` datetime NOT NULL DEFAULT '2079-01-01 12:00:00',
                              `PetID` int(11) DEFAULT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `keymaps`;
                            CREATE TABLE `keymaps` (
                              `ID` int(11) NOT NULL,
                              `CharacterID` int(11) NOT NULL,
                              `Key` int(11) NOT NULL,
                              `Type` tinyint(3) UNSIGNED NOT NULL,
                              `Action` int(11) NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `memos`;
                            CREATE TABLE `memos` (
                              `ID` int(11) NOT NULL,
                              `CharacterID` int(11) NOT NULL DEFAULT '0',
                              `Sender` varchar(13) NOT NULL DEFAULT '',
                              `Message` text,
                              `Received` datetime DEFAULT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `pets`;
                            CREATE TABLE `pets` (
                              `ID` int(11) NOT NULL,
                              `Name` varchar(13) NOT NULL DEFAULT '',
                              `Level` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
                              `Closeness` smallint(5) NOT NULL DEFAULT '0',
                              `Fullness` tinyint(3) UNSIGNED NOT NULL DEFAULT '1'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `quests_completed`;
                            CREATE TABLE `quests_completed` (
                              `CharacterID` int(11) NOT NULL,
                              `QuestID` smallint(6) UNSIGNED NOT NULL,
                              `CompletionTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `quests_started`;
                            CREATE TABLE `quests_started` (
                              `CharacterID` int(11) NOT NULL,
                              `QuestID` smallint(6) UNSIGNED NOT NULL,
                              `MobID` int(11) NOT NULL DEFAULT '0',
                              `Killed` smallint(6) NOT NULL DEFAULT '0'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `skills`;
                            CREATE TABLE `skills` (
                              `ID` int(11) NOT NULL,
                              `CharacterID` int(11) NOT NULL,
                              `MapleID` int(11) NOT NULL,
                              `CurrentLevel` tinyint(3) UNSIGNED NOT NULL,
                              `MaxLevel` tinyint(3) UNSIGNED NOT NULL,
                              `Expiration` datetime NOT NULL,
                              `CooldownEnd` datetime NOT NULL
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `storages`;
                            CREATE TABLE `storages` (
                              `AccountID` int(11) NOT NULL,
                              `Slots` tinyint(3) UNSIGNED NOT NULL DEFAULT '4',
                              `Meso` int(11) NOT NULL DEFAULT '0'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `trocks`;
                            CREATE TABLE `trocks` (
                              `ID` int(11) NOT NULL,
                              `CharacterID` int(11) NOT NULL,
                              `Index` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
                              `Map` int(11) NOT NULL DEFAULT '0'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            DROP TABLE IF EXISTS `variables`;
                            CREATE TABLE `variables` (
                              `CharacterID` int(11) NOT NULL,
                              `Key` varchar(13) NOT NULL DEFAULT '0',
                              `Value` varchar(13) NOT NULL DEFAULT '0'
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

                            ALTER TABLE `buffs`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `CharacterID` (`CharacterID`);

                            ALTER TABLE `characters`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `account_id` (`AccountID`),
                              ADD KEY `name` (`Name`) USING BTREE,
                              ADD KEY `GuildID` (`GuildID`);

                            ALTER TABLE `guilds`
                              ADD PRIMARY KEY (`ID`);

                            ALTER TABLE `items`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `character_id` (`CharacterID`) USING BTREE,
                              ADD KEY `AccountID` (`AccountID`),
                              ADD KEY `PetID` (`PetID`);

                            ALTER TABLE `keymaps`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `CharacterID` (`CharacterID`);

                            ALTER TABLE `memos`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `CharacterID` (`CharacterID`);

                            ALTER TABLE `pets`
                              ADD PRIMARY KEY (`ID`);

                            ALTER TABLE `quests_completed`
                              ADD UNIQUE KEY `Quest` (`CharacterID`,`QuestID`);

                            ALTER TABLE `quests_started`
                              ADD UNIQUE KEY `QuestRequirement` (`CharacterID`,`QuestID`,`MobID`) USING BTREE;

                            ALTER TABLE `skills`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `character_id` (`CharacterID`) USING BTREE;

                            ALTER TABLE `storages`
                              ADD PRIMARY KEY (`AccountID`) USING BTREE,
                              ADD KEY `account_id` (`AccountID`) USING BTREE;

                            ALTER TABLE `trocks`
                              ADD PRIMARY KEY (`ID`),
                              ADD KEY `character_id` (`CharacterID`) USING BTREE;

                            ALTER TABLE `variables`
                              ADD PRIMARY KEY (`CharacterID`,`Key`);

                            ALTER TABLE `buffs`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `characters`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `guilds`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `items`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `keymaps`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `memos`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `pets`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `skills`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;
                            ALTER TABLE `trocks`
                              MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

                            ALTER TABLE `buffs`
                              ADD CONSTRAINT `buffs_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `characters`
                              ADD CONSTRAINT `characters_ibfk_2` FOREIGN KEY (`GuildID`) REFERENCES `guilds` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `items`
                              ADD CONSTRAINT `items_ibfk_2` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
                              ADD CONSTRAINT `items_ibfk_3` FOREIGN KEY (`PetID`) REFERENCES `pets` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `keymaps`
                              ADD CONSTRAINT `keymaps_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `memos`
                              ADD CONSTRAINT `memos_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `quests_completed`
                              ADD CONSTRAINT `quests_completed_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `quests_started`
                              ADD CONSTRAINT `quests_started_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `skills`
                              ADD CONSTRAINT `skills_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `trocks`
                              ADD CONSTRAINT `trocks_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

                            ALTER TABLE `variables`
                              ADD CONSTRAINT `variables_ibfk_1` FOREIGN KEY (`CharacterID`) REFERENCES `characters` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;
                            ", databaseSchema);
        }
    }
}