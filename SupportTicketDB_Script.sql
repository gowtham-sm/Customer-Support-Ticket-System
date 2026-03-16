-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: supportticketdb
-- ------------------------------------------------------
-- Server version	8.0.45

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
-- Table structure for table `ticketcomments`
--

DROP TABLE IF EXISTS `ticketcomments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ticketcomments` (
  `CommentId` int NOT NULL AUTO_INCREMENT,
  `TicketId` int NOT NULL,
  `UserId` int NOT NULL,
  `CommentText` text NOT NULL,
  `IsInternal` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`CommentId`),
  KEY `TicketId` (`TicketId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ticketcomments_ibfk_1` FOREIGN KEY (`TicketId`) REFERENCES `tickets` (`TicketId`) ON DELETE CASCADE,
  CONSTRAINT `ticketcomments_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ticketcomments`
--

LOCK TABLES `ticketcomments` WRITE;
/*!40000 ALTER TABLE `ticketcomments` DISABLE KEYS */;
/*!40000 ALTER TABLE `ticketcomments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tickets`
--

DROP TABLE IF EXISTS `tickets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tickets` (
  `TicketId` int NOT NULL AUTO_INCREMENT,
  `Subject` varchar(200) NOT NULL,
  `Description` text NOT NULL,
  `Priority` enum('Low','Medium','High') NOT NULL,
  `Status` enum('Open','In Progress','Closed') NOT NULL DEFAULT 'Open',
  `CreatedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CreatedByUserId` int NOT NULL,
  `AssignedToUserId` int DEFAULT NULL,
  PRIMARY KEY (`TicketId`),
  KEY `CreatedByUserId` (`CreatedByUserId`),
  KEY `AssignedToUserId` (`AssignedToUserId`),
  CONSTRAINT `tickets_ibfk_1` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`UserId`) ON DELETE RESTRICT,
  CONSTRAINT `tickets_ibfk_2` FOREIGN KEY (`AssignedToUserId`) REFERENCES `users` (`UserId`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tickets`
--

LOCK TABLES `tickets` WRITE;
/*!40000 ALTER TABLE `tickets` DISABLE KEYS */;
/*!40000 ALTER TABLE `tickets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ticketstatushistory`
--

DROP TABLE IF EXISTS `ticketstatushistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ticketstatushistory` (
  `HistoryId` int NOT NULL AUTO_INCREMENT,
  `TicketId` int NOT NULL,
  `ChangedByUserId` int NOT NULL,
  `OldStatus` enum('Open','In Progress','Closed') DEFAULT NULL,
  `NewStatus` enum('Open','In Progress','Closed') NOT NULL,
  `ChangedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`HistoryId`),
  KEY `TicketId` (`TicketId`),
  KEY `ChangedByUserId` (`ChangedByUserId`),
  CONSTRAINT `ticketstatushistory_ibfk_1` FOREIGN KEY (`TicketId`) REFERENCES `tickets` (`TicketId`) ON DELETE CASCADE,
  CONSTRAINT `ticketstatushistory_ibfk_2` FOREIGN KEY (`ChangedByUserId`) REFERENCES `users` (`UserId`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ticketstatushistory`
--

LOCK TABLES `ticketstatushistory` WRITE;
/*!40000 ALTER TABLE `ticketstatushistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `ticketstatushistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` enum('User','Admin') NOT NULL DEFAULT 'User',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin_user','password123','Admin','2026-03-14 14:13:44'),(2,'standard_user','password123','User','2026-03-14 14:13:47'),(3,'test_admin','password@123','Admin','2026-03-16 15:21:54'),(4,'test_user','password@123','User','2026-03-16 15:22:06'),(5,'user','pass','Admin','2026-03-16 15:22:07'),(6,'admin','pass','User','2026-03-16 15:22:07');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-16 15:24:00
