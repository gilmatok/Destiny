-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: destiny
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `accounts` (
  `account_id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(20) NOT NULL,
  `password` char(130) NOT NULL DEFAULT '',
  `salt` blob,
  PRIMARY KEY (`account_id`),
  KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `characters`
--

DROP TABLE IF EXISTS `characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `characters` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `items`
--

DROP TABLE IF EXISTS `items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
  `pet_id` bigint(23) unsigned DEFAULT NULL,
  `name` varchar(12) DEFAULT NULL,
  `expiration` datetime DEFAULT NULL,
  PRIMARY KEY (`character_id`,`inventory`,`slot`),
  KEY `pet_id` (`pet_id`),
  CONSTRAINT `items_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`character_id`) ON DELETE CASCADE,
  CONSTRAINT `items_ibfk_2` FOREIGN KEY (`pet_id`) REFERENCES `pets` (`pet_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pets`
--

DROP TABLE IF EXISTS `pets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pets` (
  `pet_id` bigint(23) unsigned NOT NULL AUTO_INCREMENT,
  `index` tinyint(3) DEFAULT NULL,
  `name` varchar(12) NOT NULL,
  `level` tinyint(3) NOT NULL DEFAULT '1',
  `closeness` smallint(6) NOT NULL DEFAULT '0',
  `fullness` tinyint(3) NOT NULL DEFAULT '1',
  PRIMARY KEY (`pet_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-05-30 12:39:27
