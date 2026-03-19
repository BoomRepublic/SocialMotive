-- Migration 007: Add VerificationCodes table
-- General-purpose verification codes (email, password reset, Telegram link, etc.)

USE [SocialMotive]
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'VerificationCodes')
BEGIN
    CREATE TABLE [dbo].[VerificationCodes](
        [VerificationCodeId] [int] IDENTITY(1,1) NOT NULL,
        [UserId] [int] NULL,
        [Code] [nvarchar](255) NOT NULL,
        [CodeType] [nvarchar](50) NOT NULL,
        [Target] [nvarchar](255) NULL,
        [ExpiresAt] [datetime] NOT NULL,
        [IsUsed] [bit] NOT NULL CONSTRAINT [DF_VerificationCodes_IsUsed] DEFAULT ((0)),
        [UsedAt] [datetime] NULL,
        [CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_VerificationCodes_CreatedAt] DEFAULT (getdate()),
     CONSTRAINT [PK_VerificationCodes] PRIMARY KEY CLUSTERED 
    (
        [VerificationCodeId] ASC
    ) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[VerificationCodes] WITH CHECK ADD CONSTRAINT [FK_VerificationCodes_Users] FOREIGN KEY([UserId])
    REFERENCES [dbo].[Users] ([UserId])
    ON DELETE SET NULL

    ALTER TABLE [dbo].[VerificationCodes] CHECK CONSTRAINT [FK_VerificationCodes_Users]

    CREATE NONCLUSTERED INDEX [IX_VerificationCodes_Code] ON [dbo].[VerificationCodes] ([Code])
    CREATE NONCLUSTERED INDEX [IX_VerificationCodes_UserId] ON [dbo].[VerificationCodes] ([UserId])
    CREATE NONCLUSTERED INDEX [IX_VerificationCodes_CodeType] ON [dbo].[VerificationCodes] ([CodeType])
    CREATE NONCLUSTERED INDEX [IX_VerificationCodes_ExpiresAt] ON [dbo].[VerificationCodes] ([ExpiresAt])

    PRINT 'Created table [dbo].[VerificationCodes] with indexes and constraints.'
END
ELSE
BEGIN
    PRINT 'Table [dbo].[VerificationCodes] already exists. Skipping.'
END
GO
