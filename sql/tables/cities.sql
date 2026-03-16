USE [SocialMotive]
GO

/****** Object:  Table [dbo].[Cities]    Script Date: 14/03/2026 22:44:33 ******/
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

