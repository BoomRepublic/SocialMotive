USE [SocialMotive]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VerificationCodes](
	[VerificationCodeId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Code] [nvarchar](255) NOT NULL,
	[CodeType] [nvarchar](50) NOT NULL,
	[Target] [nvarchar](255) NULL,
	[ExpiresAt] [datetime] NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedAt] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_VerificationCodes] PRIMARY KEY CLUSTERED 
(
	[VerificationCodeId] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VerificationCodes] ADD DEFAULT ((0)) FOR [IsUsed]
GO

ALTER TABLE [dbo].[VerificationCodes] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[VerificationCodes] WITH CHECK ADD CONSTRAINT [FK_VerificationCodes_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[VerificationCodes] CHECK CONSTRAINT [FK_VerificationCodes_Users]
GO
