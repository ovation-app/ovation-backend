-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: ovation_db
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `archway_collections`
--

DROP TABLE IF EXISTS `archway_collections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `archway_collections` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractAddress` varchar(75) NOT NULL,
  `CollectionName` varchar(45) NOT NULL,
  `Image` text NOT NULL,
  `FloorPrice` decimal(12,5) NOT NULL,
  `Supply` int NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `badges`
--

DROP TABLE IF EXISTS `badges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `badges` (
  `BadgeId` binary(16) NOT NULL,
  `BadgeName` varchar(35) NOT NULL,
  `Description` text,
  `ImageUrl` text,
  `Order` tinyint NOT NULL,
  `Active` int NOT NULL DEFAULT '1',
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`BadgeId`),
  UNIQUE KEY `BadgeName_UNIQUE` (`BadgeName`),
  KEY `badge_name` (`BadgeName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `blue_chip`
--

DROP TABLE IF EXISTS `blue_chip`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `blue_chip` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CollectionName` varchar(65) NOT NULL,
  `NftCount` varchar(20) DEFAULT NULL,
  `ImageUrl` mediumtext,
  `ContractAddress` varchar(65) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `contract_idx` (`ContractAddress`) /*!80000 INVISIBLE */,
  KEY `contractNa_idx` (`CollectionName`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chain_rates`
--

DROP TABLE IF EXISTS `chain_rates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chain_rates` (
  `symbol` varchar(15) NOT NULL,
  `usd_rate` decimal(18,8) DEFAULT NULL,
  PRIMARY KEY (`symbol`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dapp_radar_collection_data`
--

DROP TABLE IF EXISTS `dapp_radar_collection_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dapp_radar_collection_data` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CollectionId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Link` mediumtext,
  `Logo` mediumtext,
  `DappId` int DEFAULT NULL,
  `AveragePrice` varchar(45) DEFAULT NULL,
  `Volume` varchar(45) DEFAULT NULL,
  `Sales` int DEFAULT NULL,
  `Metadata` json DEFAULT NULL,
  `FloorPrice` varchar(45) DEFAULT NULL,
  `Traders` varchar(45) DEFAULT NULL,
  `MarketCap` varchar(45) DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=56363 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `developer_tokens`
--

DROP TABLE IF EXISTS `developer_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `developer_tokens` (
  `TokenId` binary(16) NOT NULL,
  `FirstName` varchar(65) NOT NULL,
  `LastName` varchar(65) NOT NULL,
  `Email` varchar(45) NOT NULL,
  `CountryCode` varchar(5) NOT NULL,
  `Phone` varchar(15) NOT NULL,
  `City` varchar(65) NOT NULL,
  `Country` varchar(45) NOT NULL,
  `Platform` varchar(15) NOT NULL,
  `Active` tinyint NOT NULL DEFAULT '1',
  `Role` tinyint NOT NULL DEFAULT '3',
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`TokenId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dump_data`
--

DROP TABLE IF EXISTS `dump_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dump_data` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `data` json DEFAULT NULL,
  `Type` varchar(45) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `milestone_tasks`
--

DROP TABLE IF EXISTS `milestone_tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `milestone_tasks` (
  `TaskId` int NOT NULL AUTO_INCREMENT,
  `TaskName` varchar(35) NOT NULL,
  `Description` text,
  `MilestonesName` varchar(35) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`TaskId`),
  UNIQUE KEY `TaskName_UNIQUE` (`TaskName`),
  KEY `task_name` (`TaskName`),
  KEY `milestone_task_idx` (`MilestonesName`),
  CONSTRAINT `milestone_task` FOREIGN KEY (`MilestonesName`) REFERENCES `milestones` (`MilestoneName`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `milestones`
--

DROP TABLE IF EXISTS `milestones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `milestones` (
  `MilestoneId` int NOT NULL AUTO_INCREMENT,
  `MilestoneName` varchar(35) NOT NULL,
  `Description` text,
  `BadgeName` varchar(35) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`MilestoneId`),
  UNIQUE KEY `MilestoneName_UNIQUE` (`MilestoneName`),
  KEY `milestone_badge_idx` (`BadgeName`) /*!80000 INVISIBLE */,
  KEY `milestone_name_idx` (`MilestoneName`),
  CONSTRAINT `badge_milestone` FOREIGN KEY (`BadgeName`) REFERENCES `badges` (`BadgeName`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `newsletter`
--

DROP TABLE IF EXISTS `newsletter`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `newsletter` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SubscriberEmail` varchar(115) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `SubscriberEmail_UNIQUE` (`SubscriberEmail`)
) ENGINE=InnoDB AUTO_INCREMENT=1043 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nft_collections_data`
--

DROP TABLE IF EXISTS `nft_collections_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nft_collections_data` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ItemTotal` int DEFAULT NULL,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `ContractName` varchar(155) DEFAULT NULL,
  `ContractAddress` varchar(155) DEFAULT NULL,
  `LogoUrl` longtext,
  `Description` text,
  `Symbol` varchar(228) DEFAULT NULL,
  `Verified` tinyint DEFAULT NULL,
  `Spam` tinyint DEFAULT NULL,
  `MetaData` json DEFAULT NULL,
  `Chain` varchar(10) DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `contractAddr_idx` (`ContractAddress`),
  KEY `contractName_idx` (`ContractName`),
  KEY `itemTotal_idx` (`ItemTotal`),
  KEY `chain_idx` (`Chain`),
  KEY `symbol_idx` (`Symbol`)
) ENGINE=InnoDB AUTO_INCREMENT=5136 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nfts_data`
--

DROP TABLE IF EXISTS `nfts_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfts_data` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `TokenAddress` varchar(155) DEFAULT NULL,
  `TokenId` varchar(155) DEFAULT NULL,
  `ContractAddress` varchar(155) DEFAULT NULL,
  `MinterAddress` varchar(155) DEFAULT NULL,
  `Type` varchar(25) DEFAULT NULL,
  `Description` text,
  `ImageUrl` text,
  `AnimationUrl` text,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `MintPrice` varchar(50) DEFAULT NULL,
  `LastTradePrice` varchar(50) DEFAULT NULL,
  `LastTradeSymbol` varchar(15) DEFAULT NULL,
  `CNft` tinyint DEFAULT NULL,
  `MetaData` json DEFAULT NULL,
  `CollectionId` bigint DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `collectionId_idx` (`CollectionId`),
  KEY `name_idx` (`Name`),
  KEY `tokenId_idx` (`TokenId`),
  KEY `tokenAddress_idx` (`TokenAddress`),
  KEY `contractAddress_idx` (`ContractAddress`),
  KEY `chain_idx` (`Type`),
  KEY `dateAdded_idx` (`CreatedDate`),
  KEY `dateUpdt_idx` (`UpdatedDate`),
  KEY `price_idx` (`LastTradePrice`),
  CONSTRAINT `collectionId` FOREIGN KEY (`CollectionId`) REFERENCES `nft_collections_data` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=21511308 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `path_types`
--

DROP TABLE IF EXISTS `path_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `path_types` (
  `Id` binary(16) NOT NULL,
  `Name` varchar(65) NOT NULL,
  `Description` text NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_affilation`
--

DROP TABLE IF EXISTS `user_affilation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_affilation` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(15) NOT NULL,
  `Invited` mediumint NOT NULL DEFAULT '0',
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Code_UNIQUE` (`Code`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  KEY `affilationUser_idx` (`UserId`),
  CONSTRAINT `affilationUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=921 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_badges`
--

DROP TABLE IF EXISTS `user_badges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_badges` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EarnedAt` datetime NOT NULL,
  `Active` tinyint NOT NULL,
  `BadgeName` varchar(35) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `user_ref_idx` (`UserId`),
  KEY `badge_ref_idx` (`BadgeName`),
  CONSTRAINT `badge_ref` FOREIGN KEY (`BadgeName`) REFERENCES `badges` (`BadgeName`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `user_badge_ref` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1541 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_blue_chips`
--

DROP TABLE IF EXISTS `user_blue_chips`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_blue_chips` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `BluechipId` int NOT NULL,
  `UserWalletId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `bluchipUser_idx` (`UserId`),
  KEY `bluechip_idx` (`BluechipId`),
  KEY `blueWallet_idx` (`UserWalletId`),
  CONSTRAINT `bluchipUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `bluechip` FOREIGN KEY (`BluechipId`) REFERENCES `blue_chip` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `blueWallet` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_experiences`
--

DROP TABLE IF EXISTS `user_experiences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_experiences` (
  `Id` binary(16) NOT NULL,
  `Company` varchar(105) NOT NULL,
  `Role` varchar(75) NOT NULL,
  `Department` varchar(75) NOT NULL,
  `StartDate` date DEFAULT NULL,
  `EndDate` date DEFAULT NULL,
  `Description` text,
  `Skill` varchar(75) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `experience_user_idx` (`UserId`),
  CONSTRAINT `experience_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_favorite_nfts`
--

DROP TABLE IF EXISTS `user_favorite_nfts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_favorite_nfts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FavoriteNfts` json DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  KEY `favorite_nft_user_idx` (`UserId`),
  CONSTRAINT `favorite_nft_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=987 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_featured_items`
--

DROP TABLE IF EXISTS `user_featured_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_featured_items` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Featured` json DEFAULT NULL,
  `ItemsType` varchar(30) DEFAULT NULL,
  `ItemId` varchar(45) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `featured_user_idx` (`UserId`),
  CONSTRAINT `featured_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=985 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_feedbacks`
--

DROP TABLE IF EXISTS `user_feedbacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_feedbacks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Satisfactory` varchar(255) DEFAULT NULL,
  `UsefulFeature` varchar(255) DEFAULT NULL,
  `Improvement` text,
  `Confusion` text,
  `LikelyRecommend` varchar(45) DEFAULT NULL,
  `Addition` text,
  `BiggestPain` text,
  `UserEmail` varchar(75) NOT NULL,
  `UserId` binary(16) DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `feedback_user_idx` (`UserId`),
  CONSTRAINT `feedback_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE SET NULL ON UPDATE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_followers`
--

DROP TABLE IF EXISTS `user_followers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_followers` (
  `FollowId` binary(16) NOT NULL,
  `FollowerId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`FollowId`),
  KEY `follower_user_idx` (`FollowerId`),
  KEY `user_ref_idx` (`UserId`),
  CONSTRAINT `follower_user` FOREIGN KEY (`FollowerId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `user_ref` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_highest_nfts`
--

DROP TABLE IF EXISTS `user_highest_nfts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_highest_nfts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` text,
  `ImageUrl` longtext,
  `Worth` decimal(19,9) DEFAULT NULL,
  `Usd` decimal(19,2) DEFAULT NULL,
  `TradeSymbol` varchar(15) DEFAULT NULL,
  `Chain` varchar(15) DEFAULT NULL,
  `NftId` bigint NOT NULL,
  `WalletId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `highWallet_idx` (`WalletId`),
  KEY `highUser_idx` (`UserId`),
  KEY `highNft` (`NftId`),
  CONSTRAINT `highNft` FOREIGN KEY (`NftId`) REFERENCES `user_nft_data` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `highUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `highWallet` FOREIGN KEY (`WalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=168 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_activities`
--

DROP TABLE IF EXISTS `user_nft_activities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_activities` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EventDetails` json DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_collection_data`
--

DROP TABLE IF EXISTS `user_nft_collection_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_collection_data` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CollectionId` binary(16) DEFAULT NULL,
  `OwnsTotal` int DEFAULT NULL,
  `ItemTotal` int DEFAULT NULL,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `ContractName` varchar(155) DEFAULT NULL,
  `ContractAddress` varchar(155) DEFAULT NULL,
  `LogoUrl` longtext,
  `Description` text,
  `Symbol` varchar(228) DEFAULT NULL,
  `Verified` tinyint DEFAULT NULL,
  `Created` tinyint NOT NULL DEFAULT '0',
  `Spam` tinyint DEFAULT NULL,
  `Chain` varchar(10) DEFAULT NULL,
  `CustodyDate` date DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `UserWalletId` binary(16) NOT NULL,
  `ParentCollection` bigint DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `collection_user_idx` (`UserId`),
  KEY `collection_wallet_idx` (`UserWalletId`),
  KEY `floorPrice_idx` (`FloorPrice`),
  KEY `ownsTotal_idx` (`OwnsTotal`),
  KEY `itemTotal_idx` (`ItemTotal`),
  KEY `contractName_idx` (`ContractName`),
  KEY `contractAddr_idx` (`ContractAddress`),
  KEY `symbol_idx` (`Symbol`) /*!80000 INVISIBLE */,
  KEY `verified_idx` (`Verified`) /*!80000 INVISIBLE */,
  KEY `spam_idx` (`Spam`) /*!80000 INVISIBLE */,
  KEY `parent_collectionn_idx` (`ParentCollection`),
  KEY `created_idx` (`Created`),
  CONSTRAINT `collection_userr` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `collection_wallett` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `parent_collectionn` FOREIGN KEY (`ParentCollection`) REFERENCES `nft_collections_data` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15265 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_collections`
--

DROP TABLE IF EXISTS `user_nft_collections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_collections` (
  `Id` binary(16) NOT NULL,
  `OwnsTotal` int DEFAULT NULL,
  `ItemTotal` int DEFAULT NULL,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `ContractName` varchar(155) DEFAULT NULL,
  `ContractAddress` varchar(65) DEFAULT NULL,
  `LogoUrl` text,
  `Description` text,
  `Symbol` varchar(38) DEFAULT NULL,
  `Verified` tinyint DEFAULT NULL,
  `Spam` tinyint DEFAULT NULL,
  `Chain` varchar(10) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `UserWalletId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `collection_user_idx` (`UserId`),
  KEY `collection_wallet_idx` (`UserWalletId`),
  KEY `floorPrice_idx` (`FloorPrice`) /*!80000 INVISIBLE */,
  KEY `ownsTotal_idx` (`OwnsTotal`),
  KEY `itemTotal_idx` (`ItemTotal`) /*!80000 INVISIBLE */,
  KEY `contractName_idx` (`ContractName`) /*!80000 INVISIBLE */,
  KEY `contractAddr_idx` (`ContractAddress`) /*!80000 INVISIBLE */,
  KEY `symbol_idx` (`Symbol`) /*!80000 INVISIBLE */,
  KEY `verified_idx` (`Verified`) /*!80000 INVISIBLE */,
  KEY `spam_idx` (`Spam`) /*!80000 INVISIBLE */,
  CONSTRAINT `collection_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `collection_wallet` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_data`
--

DROP TABLE IF EXISTS `user_nft_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_data` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text,
  `TokenAddress` varchar(105) DEFAULT NULL,
  `TokenId` varchar(105) DEFAULT NULL,
  `ContractAddress` varchar(155) DEFAULT NULL,
  `Chain` varchar(25) DEFAULT NULL,
  `Description` text,
  `ImageUrl` longtext,
  `AnimationUrl` text,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `LastTradePrice` varchar(50) DEFAULT NULL,
  `LastTradeSymbol` varchar(15) DEFAULT NULL,
  `MetaData` json DEFAULT NULL,
  `Public` tinyint NOT NULL DEFAULT '1',
  `Favorite` tinyint NOT NULL DEFAULT '0',
  `Created` tinyint NOT NULL DEFAULT '0',
  `ForSale` tinyint(1) DEFAULT '0',
  `CustodyDate` date DEFAULT NULL,
  `NftId` bigint DEFAULT NULL,
  `CollectionId` bigint DEFAULT NULL,
  `UserWalletId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `nft_user_idx` (`UserId`),
  KEY `nft_wallet_idx` (`UserWalletId`),
  KEY `floorPrice_idx` (`FloorPrice`) /*!80000 INVISIBLE */,
  KEY `tradeSym_idx` (`LastTradeSymbol`) /*!80000 INVISIBLE */,
  KEY `tradePrice` (`LastTradePrice`),
  KEY `nft_coll_idx` (`CollectionId`),
  KEY `nftToken_idx` (`TokenId`),
  KEY `nftTokAddr_idx` (`TokenAddress`),
  KEY `nftContrAddr_idx` (`ContractAddress`),
  KEY `nftPublic` (`Public`),
  KEY `nftCreated` (`Created`),
  KEY `nftFav_idx` (`Favorite`),
  KEY `nft_nft` (`NftId`),
  KEY `idx_nft_for_sale` (`ForSale`),
  KEY `idx_nft_user_for_sale` (`UserId`,`ForSale`),
  FULLTEXT KEY `nftName_idx` (`Name`),
  CONSTRAINT `nft_collection` FOREIGN KEY (`CollectionId`) REFERENCES `user_nft_collection_data` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nft_nft` FOREIGN KEY (`NftId`) REFERENCES `nfts_data` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `nft_userr` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nft_wallett` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=31175 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_record`
--

DROP TABLE IF EXISTS `user_nft_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_record` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `NftCount` int NOT NULL,
  `Chain` varchar(15) DEFAULT NULL,
  `Address` varchar(65) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `nftUserRef_idx` (`UserId`),
  CONSTRAINT `nftUserRef` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=446 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_sales`
--

DROP TABLE IF EXISTS `user_nft_sales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_sales` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `SalePrice` decimal(19,9) NOT NULL,
  `SaleCurrency` varchar(10) NOT NULL,
  `SaleUrl` text NOT NULL,
  `Metadata` json DEFAULT NULL,
  `NftId` bigint NOT NULL,
  `UserId` binary(16) NOT NULL,
  `SaleCreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `SaleUpdatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `idx_nft_sale` (`NftId`),
  KEY `idx_user_sale` (`UserId`),
  CONSTRAINT `user_nft_sales_ibfk_1` FOREIGN KEY (`NftId`) REFERENCES `user_nft_data` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `user_nft_sales_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nft_transactions`
--

DROP TABLE IF EXISTS `user_nft_transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nft_transactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `TranxId` varchar(225) DEFAULT NULL,
  `ContractAddress` varchar(255) DEFAULT NULL,
  `ContractName` varchar(255) DEFAULT NULL,
  `ContractTokenId` varchar(255) DEFAULT NULL,
  `Name` varchar(155) DEFAULT NULL,
  `Image` mediumtext,
  `TokenId` varchar(255) DEFAULT NULL,
  `EventType` enum('Mint','Sale','Transfer','Burn') DEFAULT NULL,
  `To` varchar(155) DEFAULT NULL,
  `From` varchar(155) DEFAULT NULL,
  `Qty` bigint DEFAULT NULL,
  `TradePrice` varchar(50) DEFAULT NULL,
  `TradeSymbol` varchar(15) DEFAULT NULL,
  `Fee` varchar(65) DEFAULT NULL,
  `ExchangeName` varchar(75) DEFAULT NULL,
  `TranxDate` datetime DEFAULT NULL,
  `Data` json DEFAULT NULL,
  `Chain` varchar(65) NOT NULL,
  `UserWalletId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `tranxUser_idx` (`UserId`) /*!80000 INVISIBLE */,
  KEY `tranxWallet_idx` (`UserWalletId`),
  KEY `tranx_idx` (`TranxId`) /*!80000 INVISIBLE */,
  KEY `event_idx` (`EventType`) /*!80000 INVISIBLE */,
  KEY `to_idx` (`To`) /*!80000 INVISIBLE */,
  KEY `from_idx` (`From`) /*!80000 INVISIBLE */,
  KEY `name_idx` (`Name`) /*!80000 INVISIBLE */,
  KEY `contractName_idx` (`ContractName`) /*!80000 INVISIBLE */,
  KEY `fee_idx` (`Fee`) /*!80000 INVISIBLE */,
  KEY `trade_idx` (`TradePrice`) /*!80000 INVISIBLE */,
  KEY `symb_idx` (`TradeSymbol`) /*!80000 INVISIBLE */,
  KEY `chain_idx` (`Chain`) /*!80000 INVISIBLE */,
  KEY `date_idx` (`TranxDate`) /*!80000 INVISIBLE */,
  KEY `tokenId_idx` (`TokenId`) /*!80000 INVISIBLE */,
  KEY `contrackToken_idx` (`ContractTokenId`) /*!80000 INVISIBLE */,
  KEY `contractAddress_idx` (`ContractAddress`) /*!80000 INVISIBLE */,
  CONSTRAINT `tranxUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `tranxWallet` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=50773 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_nfts`
--

DROP TABLE IF EXISTS `user_nfts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_nfts` (
  `Id` binary(16) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `TokenAddress` varchar(105) DEFAULT NULL,
  `TokenId` varchar(105) DEFAULT NULL,
  `MinterAddress` varchar(105) DEFAULT NULL,
  `Type` varchar(25) DEFAULT NULL,
  `Description` text,
  `ImageUrl` text,
  `AnimationUrl` text,
  `FloorPrice` varchar(50) DEFAULT NULL,
  `MintPrice` varchar(50) DEFAULT NULL,
  `LastTradePrice` varchar(50) DEFAULT NULL,
  `LastTradeSymbol` varchar(15) DEFAULT NULL,
  `CNft` tinyint DEFAULT NULL,
  `MetaData` json DEFAULT NULL,
  `Public` tinyint NOT NULL DEFAULT '1',
  `Verified` tinyint NOT NULL,
  `CollectionId` binary(16) DEFAULT NULL,
  `UserWalletId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `nft_user_idx` (`UserId`),
  KEY `nft_minter_addr` (`MinterAddress`),
  KEY `nft_wallet_idx` (`UserWalletId`),
  KEY `cnft_idx` (`CNft`) /*!80000 INVISIBLE */,
  KEY `floorPrice_idx` (`FloorPrice`) /*!80000 INVISIBLE */,
  KEY `mintPrice_idx` (`MintPrice`) /*!80000 INVISIBLE */,
  KEY `tradeSym_idx` (`LastTradeSymbol`) /*!80000 INVISIBLE */,
  KEY `tradePrice` (`LastTradePrice`),
  KEY `nft_coll_idx` (`CollectionId`),
  CONSTRAINT `nft_coll` FOREIGN KEY (`CollectionId`) REFERENCES `user_nft_collections` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nft_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nft_wallet` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_notifications`
--

DROP TABLE IF EXISTS `user_notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_notifications` (
  `Id` binary(16) NOT NULL,
  `Reference` varchar(45) NOT NULL,
  `ReferenceId` varchar(45) DEFAULT NULL,
  `Title` varchar(45) NOT NULL,
  `Message` text NOT NULL,
  `Viewed` tinyint NOT NULL DEFAULT '0',
  `InitiatorId` binary(16) DEFAULT NULL,
  `ReceiverId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `notification_initiator_idx` (`InitiatorId`),
  KEY `notification_receiver_idx` (`ReceiverId`),
  CONSTRAINT `notification_initiator` FOREIGN KEY (`InitiatorId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `notification_receiver` FOREIGN KEY (`ReceiverId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_path_record`
--

DROP TABLE IF EXISTS `user_path_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_path_record` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PathId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `recUser_idx` (`UserId`),
  KEY `recPath_idx` (`PathId`),
  CONSTRAINT `recPath` FOREIGN KEY (`PathId`) REFERENCES `path_types` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `recUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=742 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_paths`
--

DROP TABLE IF EXISTS `user_paths`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_paths` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PathId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  KEY `path_user_idx` (`UserId`),
  KEY `path_ref_idx` (`PathId`),
  CONSTRAINT `path_ref` FOREIGN KEY (`PathId`) REFERENCES `path_types` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `path_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=987 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_portfolio_record`
--

DROP TABLE IF EXISTS `user_portfolio_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_portfolio_record` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Value` varchar(50) NOT NULL,
  `UsdValue` decimal(32,2) DEFAULT NULL,
  `MultiValue` json DEFAULT NULL,
  `Chain` varchar(15) DEFAULT NULL,
  `Address` varchar(65) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `userPort_idx` (`UserId`),
  CONSTRAINT `userPort` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2519 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_profile_views`
--

DROP TABLE IF EXISTS `user_profile_views`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_profile_views` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ViewerId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `user_viewer_idx` (`ViewerId`),
  KEY `user_idx` (`UserId`),
  CONSTRAINT `user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `user_viewer` FOREIGN KEY (`ViewerId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_profiles`
--

DROP TABLE IF EXISTS `user_profiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_profiles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DisplayName` varchar(155) NOT NULL,
  `BirthDate` date DEFAULT NULL,
  `Location` varchar(255) DEFAULT NULL,
  `Bio` text,
  `ProfileImage` text,
  `CoverImage` text,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  CONSTRAINT `profile_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=987 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_referrals`
--

DROP TABLE IF EXISTS `user_referrals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_referrals` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `InviteeId` binary(16) NOT NULL,
  `InviterId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `referralInviter_idx` (`InviterId`),
  KEY `referralInvitee_idx` (`InviteeId`),
  CONSTRAINT `referralInvitee` FOREIGN KEY (`InviteeId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `referralInviter` FOREIGN KEY (`InviterId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_socials`
--

DROP TABLE IF EXISTS `user_socials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_socials` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `LinkedIn` text,
  `Lens` text,
  `Forcaster` text,
  `Blur` text,
  `Foundation` text,
  `Twitter` text,
  `Magic` text,
  `Ethico` text,
  `Website` text,
  `Socials` json DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  CONSTRAINT `social_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=987 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_stats`
--

DROP TABLE IF EXISTS `user_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_stats` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Followers` bigint NOT NULL DEFAULT '0',
  `XFollowers` bigint NOT NULL DEFAULT '0',
  `Following` bigint NOT NULL DEFAULT '0',
  `XFollowing` bigint NOT NULL DEFAULT '0',
  `NftCreated` bigint NOT NULL DEFAULT '0',
  `NftCollected` bigint NOT NULL DEFAULT '0',
  `NftCollections` bigint NOT NULL DEFAULT '0',
  `FounderNft` int NOT NULL DEFAULT '0',
  `TotalNft` bigint NOT NULL DEFAULT '0',
  `SoldNftsTotal` int NOT NULL DEFAULT '0',
  `SoldNftsValue` decimal(19,2) NOT NULL DEFAULT '0.00',
  `BadgeEarned` bigint NOT NULL DEFAULT '0',
  `Networth` decimal(22,2) NOT NULL DEFAULT '0.00',
  `BluechipCount` int NOT NULL,
  `Views` bigint NOT NULL DEFAULT '0',
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`),
  KEY `stat_user_idx` (`UserId`),
  KEY `xFollowers_idx` (`XFollowers`),
  KEY `xFollowing_idx` (`XFollowing`),
  CONSTRAINT `stat_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=987 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_tasks`
--

DROP TABLE IF EXISTS `user_tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_tasks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompletedAt` datetime NOT NULL,
  `TaskName` varchar(35) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `task_ref_idx` (`TaskName`),
  KEY `task_user_ref_idx` (`UserId`),
  CONSTRAINT `task_ref` FOREIGN KEY (`TaskName`) REFERENCES `milestone_tasks` (`TaskName`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `task_user_ref` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1985 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_wallet_group`
--

DROP TABLE IF EXISTS `user_wallet_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_wallet_group` (
  `Id` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `WalletId` binary(16) DEFAULT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `groupWallet_idx` (`WalletId`),
  KEY `userGroup` (`UserId`),
  CONSTRAINT `groupWallet` FOREIGN KEY (`WalletId`) REFERENCES `wallets` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `userGroup` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_wallet_sales_record`
--

DROP TABLE IF EXISTS `user_wallet_sales_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_wallet_sales_record` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TotalSold` int NOT NULL,
  `TotalSales` decimal(19,2) NOT NULL,
  `Chain` varchar(15) NOT NULL,
  `WalletId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `walletSales_idx` (`WalletId`),
  KEY `userSales` (`UserId`),
  CONSTRAINT `userSales` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `walletSales` FOREIGN KEY (`WalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=118 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_wallet_values`
--

DROP TABLE IF EXISTS `user_wallet_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_wallet_values` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `NftCount` int NOT NULL,
  `NativeWorth` decimal(19,8) NOT NULL,
  `Chain` varchar(15) NOT NULL,
  `UserWalletId` binary(16) NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `walletUserv_idx` (`UserId`),
  KEY `valueWallet_idx` (`UserWalletId`),
  CONSTRAINT `valueUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `valueWallet` FOREIGN KEY (`UserWalletId`) REFERENCES `user_wallets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=183 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_wallets`
--

DROP TABLE IF EXISTS `user_wallets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_wallets` (
  `Id` binary(16) NOT NULL,
  `WalletAddress` varchar(75) NOT NULL,
  `Chain` varchar(35) NOT NULL,
  `Blockchain` varchar(10) DEFAULT NULL,
  `NftsValue` varchar(50) DEFAULT NULL,
  `NftCount` int DEFAULT NULL,
  `TotalSold` int DEFAULT NULL,
  `TotalSales` decimal(19,2) DEFAULT NULL,
  `MetaData` json DEFAULT NULL,
  `Migrated` tinyint NOT NULL DEFAULT '0',
  `WalletGroupId` binary(16) DEFAULT NULL,
  `WalletId` binary(16) DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `wallet_user_idx` (`UserId`),
  KEY `wallet_ref_idx` (`WalletId`),
  KEY `wallet_chain_idx` (`Chain`),
  KEY `wallet_blockchain_idx` (`Blockchain`),
  KEY `wallet_address_idx` (`WalletAddress`),
  KEY `wallet_group_idx` (`WalletGroupId`),
  CONSTRAINT `wallet_group` FOREIGN KEY (`WalletGroupId`) REFERENCES `user_wallet_group` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `wallet_ref` FOREIGN KEY (`WalletId`) REFERENCES `wallets` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `wallet_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserId` binary(16) NOT NULL,
  `Username` varchar(45) NOT NULL,
  `Email` varchar(75) DEFAULT NULL,
  `GoogleId` varchar(75) NOT NULL,
  `Password` text NOT NULL,
  `Type` varchar(10) NOT NULL,
  `Active` tinyint NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `username_UNIQUE` (`Username`),
  UNIQUE KEY `GoogleId_UNIQUE` (`GoogleId`),
  UNIQUE KEY `email_UNIQUE` (`Email`),
  KEY `gid_idx` (`GoogleId`) /*!80000 INVISIBLE */,
  KEY `type_idx` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `verified_users`
--

DROP TABLE IF EXISTS `verified_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `verified_users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Type` enum('X','Lens','Farcaster','ENS') NOT NULL,
  `Handle` varchar(75) NOT NULL,
  `TypeId` varchar(75) NOT NULL,
  `MetaData` json DEFAULT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `verifiedUser_idx` (`UserId`) /*!80000 INVISIBLE */,
  KEY `typeId_idx` (`TypeId`),
  KEY `handle_idx` (`Handle`),
  KEY `type_idx` (`Type`),
  CONSTRAINT `verifiedUser` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=115 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `verify_user`
--

DROP TABLE IF EXISTS `verify_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `verify_user` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserCode` binary(16) NOT NULL,
  `Otp` int NOT NULL,
  `UserId` binary(16) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `userVerify_idx` (`UserId`),
  CONSTRAINT `userVerify` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=209 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `wallets`
--

DROP TABLE IF EXISTS `wallets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wallets` (
  `Id` binary(16) NOT NULL,
  `Name` varchar(45) NOT NULL,
  `LogoUrl` text,
  `Active` tinyint NOT NULL DEFAULT '1',
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `x_target_accounts`
--

DROP TABLE IF EXISTS `x_target_accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `x_target_accounts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(65) NOT NULL,
  `Engaged` tinyint NOT NULL DEFAULT '0',
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `Username_UNIQUE` (`Username`),
  KEY `engaged_idx` (`Engaged`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-09-12 19:02:54
