-- Seed script for [dbo].[Labels]
-- Generated from data/labels.csv

SET IDENTITY_INSERT [dbo].[Labels] ON;

INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (1, N'PUBLIC', NULL, NULL, NULL, 0, 0);
INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (2, N'GAST', NULL, NULL, NULL, 0, 0);
INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (3, N'VRIJWILLIGER', NULL, NULL, NULL, 0, 0);
INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (4, N'BEZOEKER', NULL, NULL, NULL, 0, 0);
INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (5, N'PENDELBUS', NULL, NULL, NULL, 0, 0);
INSERT INTO [dbo].[Labels] (LabelId, Name, ColorHex, IconType, LabelType, Publish, Level) VALUES (6, N'TEST', N'#000000', NULL, NULL, 0, 0);

SET IDENTITY_INSERT [dbo].[Labels] OFF;
