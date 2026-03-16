USE [SocialMotive]
GO

/****** Object:  Table [dbo].[Events]    Script Date: 14/03/2026 22:45:06 ******/
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
	[OrganizerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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

ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EventTypes] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[EventTypes] ([Id])
GO

ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EventTypes]
GO

ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Users] FOREIGN KEY([OrganizerId])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Users]
GO

