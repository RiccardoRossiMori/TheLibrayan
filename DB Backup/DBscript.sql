USE [master]
GO
/****** Object:  Database [TheLibrayan]    Script Date: 10/08/2024 19:50:08 ******/
CREATE DATABASE [TheLibrayan]
    CONTAINMENT = NONE
    ON PRIMARY
    ( NAME = N'TheLibrayan', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TheLibrayan.mdf' , SIZE = 8192 KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536 KB )
    LOG ON
    ( NAME = N'TheLibrayan_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TheLibrayan_log.ldf' , SIZE = 8192 KB , MAXSIZE = 2048 GB , FILEGROWTH = 65536 KB )
WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TheLibrayan] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
    begin
        EXEC [TheLibrayan].[dbo].[sp_fulltext_database] @action = 'enable'
    end
GO
ALTER DATABASE [TheLibrayan] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [TheLibrayan] SET ANSI_NULLS OFF
GO
ALTER DATABASE [TheLibrayan] SET ANSI_PADDING OFF
GO
ALTER DATABASE [TheLibrayan] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [TheLibrayan] SET ARITHABORT OFF
GO
ALTER DATABASE [TheLibrayan] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [TheLibrayan] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [TheLibrayan] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [TheLibrayan] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [TheLibrayan] SET CURSOR_DEFAULT GLOBAL
GO
ALTER DATABASE [TheLibrayan] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [TheLibrayan] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [TheLibrayan] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [TheLibrayan] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [TheLibrayan] SET DISABLE_BROKER
GO
ALTER DATABASE [TheLibrayan] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [TheLibrayan] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [TheLibrayan] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [TheLibrayan] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [TheLibrayan] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [TheLibrayan] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [TheLibrayan] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [TheLibrayan] SET RECOVERY FULL
GO
ALTER DATABASE [TheLibrayan] SET MULTI_USER
GO
ALTER DATABASE [TheLibrayan] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [TheLibrayan] SET DB_CHAINING OFF
GO
ALTER DATABASE [TheLibrayan] SET FILESTREAM ( NON_TRANSACTED_ACCESS = OFF )
GO
ALTER DATABASE [TheLibrayan] SET TARGET_RECOVERY_TIME = 60 SECONDS
GO
ALTER DATABASE [TheLibrayan] SET DELAYED_DURABILITY = DISABLED
GO
ALTER DATABASE [TheLibrayan] SET ACCELERATED_DATABASE_RECOVERY = OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'TheLibrayan', N'ON'
GO
ALTER DATABASE [TheLibrayan] SET QUERY_STORE = ON
GO
ALTER DATABASE [TheLibrayan] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TheLibrayan]
GO
/****** Object:  User [Librayan]    Script Date: 10/08/2024 19:50:09 ******/
CREATE USER [Librayan] FOR LOGIN [Librayan] WITH DEFAULT_SCHEMA =[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [Librayan]
GO
/****** Object:  Table [dbo].[Categorie]    Script Date: 10/08/2024 19:50:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categorie]
(
    [CategoriaID]   [int] IDENTITY (1,1) NOT NULL,
    [NomeCategoria] [nvarchar](255)      NOT NULL,
    PRIMARY KEY CLUSTERED
        (
         [CategoriaID] ASC
            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Libri]    Script Date: 10/08/2024 19:50:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Libri]
(
    [LibroID]           [int] IDENTITY (1,1) NOT NULL,
    [Titolo]            [nvarchar](255)      NOT NULL,
    [Autore]            [nvarchar](255)      NOT NULL,
    [DataPubblicazione] [int]                NOT NULL,
    PRIMARY KEY CLUSTERED
        (
         [LibroID] ASC
            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LibriCategorie]    Script Date: 10/08/2024 19:50:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LibriCategorie]
(
    [LibroID]     [int] NOT NULL,
    [CategoriaID] [int] NOT NULL,
    PRIMARY KEY CLUSTERED
        (
         [LibroID] ASC,
         [CategoriaID] ASC
            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Utenti]    Script Date: 10/08/2024 19:50:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Utenti]
(
    [Email]    [varchar](50) NOT NULL,
    [Nome]     [varchar](50) NOT NULL,
    [Cognome]  [varchar](50) NOT NULL,
    [Password] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Utenti] PRIMARY KEY CLUSTERED
        (
         [Email] ASC
            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LibriCategorie]
    WITH CHECK ADD CONSTRAINT [FK_LibriCategorie_LibriCategorie] FOREIGN KEY ([LibroID], [CategoriaID])
        REFERENCES [dbo].[LibriCategorie] ([LibroID], [CategoriaID])
GO
ALTER TABLE [dbo].[LibriCategorie]
    CHECK CONSTRAINT [FK_LibriCategorie_LibriCategorie]
GO
USE [master]
GO
ALTER DATABASE [TheLibrayan] SET READ_WRITE
GO
-- Esegui lo script SQL per creare la struttura del database
-- Apri e esegui il contenuto di `DBscript.sql` in SSMS

-- Connettersi al server SQL
USE master;
GO

-- Ripristinare il database da un file di backup
RESTORE DATABASE TheLibrayan
FROM DISK = '[Indirizzo assoluto che porta alla Repo in questione]TheLibrayan/DB Backup/TheLibrayan.bak'
WITH REPLACE;
GO