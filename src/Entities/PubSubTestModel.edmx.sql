
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 02/19/2011 11:18:30
-- Generated from EDMX file: C:\Dev\ReferenceImplementatikon\Entities\PubSubTestModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PubSub];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Message]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message];
GO
IF OBJECT_ID(N'[dbo].[Message2]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message2];
GO
IF OBJECT_ID(N'[dbo].[Message3]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message3];
GO
IF OBJECT_ID(N'[dbo].[Message4]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message4];
GO
IF OBJECT_ID(N'[dbo].[Message5]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message5];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [MessageID] varchar(100)  NOT NULL,
    [SubscriptionID] varchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [MessagePutTime] datetime  NOT NULL,
    [Timestamp] binary(8)  NOT NULL,
    [BatchNumber] int  NOT NULL,
    [MessageReadTime] datetime  NOT NULL
);
GO

-- Creating table 'Message2'
CREATE TABLE [dbo].[Message2] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [MessageID] varchar(100)  NOT NULL,
    [SubscriptionID] varchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [MessagePutTime] datetime  NOT NULL,
    [MessageReadTime] datetime  NOT NULL,
    [Timestamp] binary(8)  NOT NULL,
    [BatchNumber] int  NOT NULL
);
GO

-- Creating table 'Message3'
CREATE TABLE [dbo].[Message3] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [MessageID] varchar(100)  NOT NULL,
    [SubscriptionID] varchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [MessagePutTime] datetime  NOT NULL,
    [MessageReadTime] datetime  NOT NULL,
    [Timestamp] binary(8)  NOT NULL,
    [BatchNumber] int  NOT NULL
);
GO

-- Creating table 'Message4'
CREATE TABLE [dbo].[Message4] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [MessageID] varchar(100)  NOT NULL,
    [SubscriptionID] varchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [MessagePutTime] datetime  NOT NULL,
    [MessageReadTime] datetime  NOT NULL,
    [Timestamp] binary(8)  NOT NULL,
    [BatchNumber] int  NOT NULL
);
GO

-- Creating table 'Message5'
CREATE TABLE [dbo].[Message5] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [MessageID] varchar(100)  NOT NULL,
    [SubscriptionID] varchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [MessagePutTime] datetime  NOT NULL,
    [MessageReadTime] datetime  NOT NULL,
    [Timestamp] binary(8)  NOT NULL,
    [BatchNumber] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Guid] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'Message2'
ALTER TABLE [dbo].[Message2]
ADD CONSTRAINT [PK_Message2]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'Message3'
ALTER TABLE [dbo].[Message3]
ADD CONSTRAINT [PK_Message3]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'Message4'
ALTER TABLE [dbo].[Message4]
ADD CONSTRAINT [PK_Message4]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'Message5'
ALTER TABLE [dbo].[Message5]
ADD CONSTRAINT [PK_Message5]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------