USE [master]
GO
/****** Object:  Database [SocialMotive]    Script Date: 11/03/2026 10:43:40 ******/
CREATE DATABASE [SocialMotive]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Trekker', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\Trekker.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Trekker_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\Trekker_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [SocialMotive] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SocialMotive].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SocialMotive] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SocialMotive] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SocialMotive] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SocialMotive] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SocialMotive] SET ARITHABORT OFF 
GO
ALTER DATABASE [SocialMotive] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SocialMotive] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SocialMotive] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SocialMotive] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SocialMotive] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SocialMotive] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SocialMotive] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SocialMotive] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SocialMotive] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SocialMotive] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SocialMotive] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SocialMotive] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SocialMotive] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SocialMotive] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SocialMotive] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SocialMotive] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SocialMotive] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SocialMotive] SET RECOVERY FULL 
GO
ALTER DATABASE [SocialMotive] SET  MULTI_USER 
GO
ALTER DATABASE [SocialMotive] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SocialMotive] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SocialMotive] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SocialMotive] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SocialMotive] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SocialMotive] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [SocialMotive] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'SocialMotive', N'ON'
GO
ALTER DATABASE [SocialMotive] SET QUERY_STORE = ON
GO
ALTER DATABASE [SocialMotive] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [SocialMotive]
GO
/****** Object:  User [web]    Script Date: 11/03/2026 10:43:40 ******/
CREATE USER [web] FOR LOGIN [web] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [web]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [web]
GO
/****** Object:  Table [dbo].[Cities]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cities](
	[CityId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Code] [nvarchar](50) NULL,
	[CodeGm] [nvarchar](50) NULL,
	[ProvincieNaam] [nvarchar](150) NULL,
	[ProvincieCode] [nvarchar](50) NULL,
	[ProvincieCodePv] [nvarchar](50) NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventParticipants]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventParticipants](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[HoursWorked] [decimal](5, 2) NULL,
	[Rating] [int] NULL,
	[Review] [nvarchar](max) NULL,
	[JoinedAt] [datetime] NOT NULL,
	[CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[EventTypeId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[OrganizerId] [uniqueidentifier] NOT NULL,
	[ProfileImage] [varbinary](max) NULL,
	[CoverImage] [varbinary](max) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Address] [nvarchar](255) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[City] [nvarchar](100) NULL,
	[MaxParticipants] [int] NULL,
	[MinParticipants] [int] NULL,
	[HoursEstimate] [decimal](5, 2) NULL,
	[SkillsRequired] [nvarchar](max) NULL,
	[BenefitsDescription] [nvarchar](max) NULL,
	[RewardPoints] [int] NULL,
	[IsVerified] [bit] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[PublishedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTaskAssignments]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventTaskAssignments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventTaskId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTasks]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Difficulty] [int] NOT NULL,
	[Required] [bit] NOT NULL,
	[MaxParticipants] [int] NULL,
	[HoursEstimate] [decimal](5, 2) NULL,
	[OrderIndex] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTypes]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventTypes](
	[Id] [int] IDENTITY(0,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Icon] [nvarchar](50) NULL,
	[Color] [nvarchar](7) NULL,
	[Created] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ColorHex] [char](7) NULL,
	[IconType] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
	[Publish] [bit] NOT NULL,
	[Level] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invites]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invites](
	[InviteId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedByTrackerId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[Name] [nvarchar](150) NULL,
	[Description] [nvarchar](500) NULL,
	[Notes] [nvarchar](max) NULL,
	[InviteType] [nvarchar](50) NULL,
	[ClaimedByTrackerId] [int] NULL,
 CONSTRAINT [PK_Invites] PRIMARY KEY CLUSTERED 
(
	[InviteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Labels]    Script Date: 11/03/2026 10:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Labels](
	[LabelId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ColorHex] [char](7) NULL,
	[IconType] [nvarchar](50) NULL,
	[LabelType] [nvarchar](25) NULL,
	[Publish] [bit] NOT NULL,
	[Level] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LabelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Locations]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Locations](
	[LocationId] [bigint] IDENTITY(1,1) NOT NULL,
	[TrackerId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[AccuracyMeters] [float] NULL,
	[AltitudeMeters] [float] NULL,
	[SpeedKmh] [float] NULL,
	[HeadingDegrees] [float] NULL,
	[Timestamp] [datetime] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[SettingKey] [nvarchar](50) NOT NULL,
	[SettingValue] [nvarchar](100) NOT NULL,
	[Scope] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SettingKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrackerLabels]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackerLabels](
	[TrackerLabelId] [int] IDENTITY(1,1) NOT NULL,
	[TrackerId] [int] NOT NULL,
	[LabelId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TrackerLabelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrackerRoles]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackerRoles](
	[TrackerRoleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TrackerRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Trackers]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Trackers](
	[TrackerId] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NULL,
	[TrackerRoleId] [int] NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](150) NULL,
	[Mobile] [nvarchar](30) NULL,
	[LicensePlate] [nvarchar](20) NULL,
	[InviteCode] [uniqueidentifier] NOT NULL,
	[InviteName] [nvarchar](50) NULL,
	[InvitedBy_TrackerId] [int] NULL,
	[QrGuid] [uniqueidentifier] NOT NULL,
	[JoinedAt] [datetime] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedAt] [datetime] NOT NULL,
	[CheckIn] [int] NOT NULL,
	[CheckInTime] [datetime] NULL,
	[CheckInLat] [float] NULL,
	[CheckInLon] [float] NULL,
	[CheckInBy_TrackerId] [int] NULL,
	[CheckInByTrackerId] [int] NULL,
	[InvitedByTrackerId] [int] NULL,
	[CityId] [int] NULL,
	[IsAdmin] [bit] NULL,
	[InviteId] [int] NULL,
	[UserId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[TrackerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Trackers_QrGuid] UNIQUE NONCLUSTERED 
(
	[QrGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Platform] [int] NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Url] [nvarchar](max) NULL,
	[Verified] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/03/2026 10:43:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[MobilePhone] [nvarchar](20) NULL,
	[ProfileImage] [varbinary](max) NULL,
	[CoverImage] [varbinary](max) NULL,
	[Bio] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_EventParticipants_EventId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventParticipants_EventId] ON [dbo].[EventParticipants]
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventParticipants_UserId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventParticipants_UserId] ON [dbo].[EventParticipants]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Events_OrganizerId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Events_OrganizerId] ON [dbo].[Events]
(
	[OrganizerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Events_StartDate]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Events_StartDate] ON [dbo].[Events]
(
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Events_Status]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Events_Status] ON [dbo].[Events]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventTaskAssignments_EventTaskId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventTaskAssignments_EventTaskId] ON [dbo].[EventTaskAssignments]
(
	[EventTaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventTaskAssignments_UserId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventTaskAssignments_UserId] ON [dbo].[EventTaskAssignments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventTasks_Difficulty]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventTasks_Difficulty] ON [dbo].[EventTasks]
(
	[Difficulty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EventTasks_EventId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_EventTasks_EventId] ON [dbo].[EventTasks]
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Locations_Timestamp]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Locations_Timestamp] ON [dbo].[Locations]
(
	[Timestamp] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Locations_Tracker_Time]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Locations_Tracker_Time] ON [dbo].[Locations]
(
	[TrackerId] ASC,
	[Timestamp] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackers_CheckIn]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_CheckIn] ON [dbo].[Trackers]
(
	[CheckIn] ASC
)
INCLUDE([CheckInTime],[CheckInLat],[CheckInLon]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Trackers_Email]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_Email] ON [dbo].[Trackers]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackers_GroupId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_GroupId] ON [dbo].[Trackers]
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackers_InvitedBy_TrackerId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_InvitedBy_TrackerId] ON [dbo].[Trackers]
(
	[InvitedBy_TrackerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Trackers_Mobile]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_Mobile] ON [dbo].[Trackers]
(
	[Mobile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackers_TrackerRoleId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_TrackerRoleId] ON [dbo].[Trackers]
(
	[TrackerRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackers_UserId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Trackers_UserId] ON [dbo].[Trackers]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAccounts_UserId]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_UserAccounts_UserId] ON [dbo].[UserAccounts]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 11/03/2026 10:43:41 ******/
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EventParticipants] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[EventParticipants] ADD  DEFAULT (getdate()) FOR [JoinedAt]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [RewardPoints]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[EventTaskAssignments] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[EventTaskAssignments] ADD  DEFAULT (getdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[EventTasks] ADD  DEFAULT ((1)) FOR [Difficulty]
GO
ALTER TABLE [dbo].[EventTasks] ADD  DEFAULT ((1)) FOR [Required]
GO
ALTER TABLE [dbo].[EventTasks] ADD  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[EventTasks] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[EventTasks] ADD  DEFAULT (getdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[EventTypes] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Groups] ADD  DEFAULT ((0)) FOR [Publish]
GO
ALTER TABLE [dbo].[Groups] ADD  DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE [dbo].[Groups] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Groups] ADD  DEFAULT (getutcdate()) FOR [ModifiedAt]
GO
ALTER TABLE [dbo].[Labels] ADD  DEFAULT ((0)) FOR [Publish]
GO
ALTER TABLE [dbo].[Labels] ADD  DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (getutcdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (getutcdate()) FOR [ModifiedAt]
GO
ALTER TABLE [dbo].[TrackerRoles] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT (newid()) FOR [InviteCode]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT (newid()) FOR [QrGuid]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT (getutcdate()) FOR [JoinedAt]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT (getutcdate()) FOR [ModifiedAt]
GO
ALTER TABLE [dbo].[Trackers] ADD  DEFAULT ((0)) FOR [CheckIn]
GO
ALTER TABLE [dbo].[UserAccounts] ADD  DEFAULT ((0)) FOR [Verified]
GO
ALTER TABLE [dbo].[UserAccounts] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[UserAccounts] ADD  DEFAULT (getdate()) FOR [Modified]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [Modified]
GO
ALTER TABLE [dbo].[EventParticipants]  WITH CHECK ADD  CONSTRAINT [FK_EventParticipants_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventParticipants] CHECK CONSTRAINT [FK_EventParticipants_Events]
GO
ALTER TABLE [dbo].[EventParticipants]  WITH CHECK ADD  CONSTRAINT [FK_EventParticipants_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventParticipants] CHECK CONSTRAINT [FK_EventParticipants_Users]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EventTypes] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[EventTypes] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EventTypes]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Users] FOREIGN KEY([OrganizerId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Users]
GO
ALTER TABLE [dbo].[EventTaskAssignments]  WITH CHECK ADD  CONSTRAINT [FK_EventTaskAssignments_EventTasks] FOREIGN KEY([EventTaskId])
REFERENCES [dbo].[EventTasks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventTaskAssignments] CHECK CONSTRAINT [FK_EventTaskAssignments_EventTasks]
GO
ALTER TABLE [dbo].[EventTaskAssignments]  WITH CHECK ADD  CONSTRAINT [FK_EventTaskAssignments_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventTaskAssignments] CHECK CONSTRAINT [FK_EventTaskAssignments_Users]
GO
ALTER TABLE [dbo].[EventTasks]  WITH CHECK ADD  CONSTRAINT [FK_EventTasks_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventTasks] CHECK CONSTRAINT [FK_EventTasks_Events]
GO
ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Trackers] FOREIGN KEY([TrackerId])
REFERENCES [dbo].[Trackers] ([TrackerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Locations] CHECK CONSTRAINT [FK_Locations_Trackers]
GO
ALTER TABLE [dbo].[TrackerLabels]  WITH CHECK ADD  CONSTRAINT [FK_TrackerLabels_Labels] FOREIGN KEY([LabelId])
REFERENCES [dbo].[Labels] ([LabelId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrackerLabels] CHECK CONSTRAINT [FK_TrackerLabels_Labels]
GO
ALTER TABLE [dbo].[TrackerLabels]  WITH CHECK ADD  CONSTRAINT [FK_TrackerLabels_Trackers] FOREIGN KEY([TrackerId])
REFERENCES [dbo].[Trackers] ([TrackerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrackerLabels] CHECK CONSTRAINT [FK_TrackerLabels_Trackers]
GO
ALTER TABLE [dbo].[Trackers]  WITH CHECK ADD  CONSTRAINT [FK_Trackers_Cities] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([CityId])
GO
ALTER TABLE [dbo].[Trackers] CHECK CONSTRAINT [FK_Trackers_Cities]
GO
ALTER TABLE [dbo].[Trackers]  WITH CHECK ADD  CONSTRAINT [FK_Trackers_Trackers_CheckInByTrackerId] FOREIGN KEY([CheckInByTrackerId])
REFERENCES [dbo].[Trackers] ([TrackerId])
GO
ALTER TABLE [dbo].[Trackers] CHECK CONSTRAINT [FK_Trackers_Trackers_CheckInByTrackerId]
GO
ALTER TABLE [dbo].[Trackers]  WITH CHECK ADD  CONSTRAINT [FK_Trackers_Trackers_InvitedByTrackerId] FOREIGN KEY([InvitedByTrackerId])
REFERENCES [dbo].[Trackers] ([TrackerId])
GO
ALTER TABLE [dbo].[Trackers] CHECK CONSTRAINT [FK_Trackers_Trackers_InvitedByTrackerId]
GO
ALTER TABLE [dbo].[Trackers]  WITH CHECK ADD  CONSTRAINT [FK_Trackers_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Trackers] CHECK CONSTRAINT [FK_Trackers_Users]
GO
ALTER TABLE [dbo].[UserAccounts]  WITH CHECK ADD  CONSTRAINT [FK_UserAccounts_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserAccounts] CHECK CONSTRAINT [FK_UserAccounts_Users]
GO
USE [master]
GO
ALTER DATABASE [SocialMotive] SET  READ_WRITE 
GO
