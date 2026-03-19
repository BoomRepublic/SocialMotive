# SQL Scripts - AI Assistant Instructions

## Overview
This folder contains SQL Server scripts for the **SocialMotive** database. The application uses **EF Core Code First** as the primary schema management approach; these scripts serve as reference, manual seeding, and one-off migration utilities.

## Database
- **Database name**: `SocialMotive`
- **Engine**: SQL Server (MSSQL 17+)
- **Schema**: `dbo` (default schema for all objects)

## Folder Structure
```
sql/
  claude.md            # This file
  create.sql           # Database creation (placeholder)
  schema.sql           # Full database creation with settings (SSMS-generated)
  tables/              # Individual CREATE TABLE scripts (one per table)
    Cities.sql
    Events.sql
    EventParticipants.sql
  seed/                # INSERT scripts for populating reference/test data
    seed-cities.sql
    seed-labels.sql
    seed-users.sql
    seed_users.sql      # Alternate format (newer schema, GETUTC timestamps)
  migrations/           # Numbered migration scripts (run sequentially)
    000-rename-pk-columns.sql
    001-add-username-to-users.sql
```

## Naming Conventions
- **Table files**: `{TableName}.sql` — PascalCase, matching the SQL table name
- **Seed files**: `seed-{tablename}.sql` — lowercase with hyphens
- **Migration scripts**: `{NNN}-{description}.sql` — zero-padded 3-digit sequence, kebab-case description (e.g., `001-add-username-to-users.sql`)
- **Table names**: PascalCase plural (e.g., `Cities`, `Events`, `EventParticipants`)
- **Column names**: PascalCase (e.g., `CityId`, `FirstName`, `EventTypeId`)
- **Primary keys**: `{SingularTableName}Id` (e.g., `CityId`, `EventId`, `LabelId`)
- **Foreign keys**: `{ReferencedTable}Id` (e.g., `EventTypeId`, `UserId`)
- **Constraint names**: `PK_{Table}` for primary keys, `FK_{Table}_{ReferencedTable}` for foreign keys

## Script Conventions

### Table Scripts
- Each file targets a single table
- Begin with `USE [SocialMotive]` and `GO`
- Include `SET ANSI_NULLS ON` and `SET QUOTED_IDENTIFIER ON`
- Define `CREATE TABLE` with full column specs and inline `PRIMARY KEY CLUSTERED`
- Add `DEFAULT` constraints via separate `ALTER TABLE ... ADD DEFAULT` statements
- Add foreign keys via separate `ALTER TABLE ... ADD CONSTRAINT` statements
- Include `WITH CHECK ADD CONSTRAINT` and matching `CHECK CONSTRAINT` for FK validation

### Seed Scripts
- Begin with a comment header: `-- Seed script for [dbo].[{Table}]`
- Include source info: `-- Generated from data/{source_file}`
- Use `SET IDENTITY_INSERT [dbo].[{Table}] ON/OFF` when inserting explicit identity values
- One `INSERT INTO` per row for clarity and easy diffing
- Use `N'...'` for all nvarchar string literals
- Use `NULL` explicitly (not empty strings) for absent values

### General
- Always qualify table names with `[dbo].[{Table}]`
- Use square bracket notation for all identifiers: `[ColumnName]`, `[TableName]`
- Separate statements with `GO` batch terminators
- `datetime` columns default to `getdate()` where applicable
- `bit` columns default to `0` unless otherwise specified
- FK delete behavior: explicitly specified (default `NO ACTION`; use `ON DELETE CASCADE` only when appropriate)

## Data Types Reference (project conventions)
| Purpose | SQL Type |
|---|---|
| Primary key (identity) | `int IDENTITY(1,1)` |
| Short text | `nvarchar(50–255)` |
| Long text | `nvarchar(max)` |
| Boolean | `bit` |
| Date/time | `datetime` |
| Decimal | `decimal(5,2)` |
| Coordinates | `float` |
| Images/binary | `varbinary(max)` |
| Enum values | `int` |

## Relationship to EF Core
- The **canonical schema** is defined by `SocialMotiveDbContext` in `src/SocialMotive.Core/Model/`
- These SQL scripts are **supplementary** — use them for manual seeding, debugging, or documenting the schema outside of EF
- When the EF model changes, table scripts here may become stale — update them manually or regenerate from SSMS
- Generator tables use explicit `ToTable(...)` mappings in the DbContext fluent API

## Schema Snapshot
> **Last updated**: 2025-07-15
> **Refresh command**: `sqlcmd -S . -d SocialMotive -E -i sql\query-schema.sql -W`

### Tables & Columns
```
TABLE_NAME COLUMN_NAME Type Nullable Identity Default
---------- ----------- ---- -------- -------- -------
Cities CityId int NOT NULL IDENTITY
Cities Name nvarchar(250) NULL
Cities Latitude float NULL
Cities Longitude float NULL
Cities Code nvarchar(50) NULL
Cities CodeGm nvarchar(50) NULL
Cities ProvincieNaam nvarchar(150) NULL
Cities ProvincieCode nvarchar(50) NULL
Cities ProvincieCodePv nvarchar(50) NULL
EventParticipants EventParticipantId int NOT NULL IDENTITY
EventParticipants EventId int NOT NULL
EventParticipants Status int NOT NULL ((0))
EventParticipants HoursWorked decimal(5,2) NULL
EventParticipants Rating int NULL
EventParticipants Review nvarchar(max) NULL
EventParticipants JoinedAt datetime NOT NULL (getdate())
EventParticipants CompletedAt datetime NULL
EventParticipants UserId int NULL
Events EventId int NOT NULL IDENTITY
Events Title nvarchar(255) NOT NULL
Events Description nvarchar(max) NOT NULL
Events EventTypeId int NOT NULL
Events Status int NOT NULL ((1))
Events ProfileImage varbinary(max) NULL
Events CoverImage varbinary(max) NULL
Events StartDate datetime NOT NULL
Events EndDate datetime NOT NULL
Events Address nvarchar(255) NULL
Events Latitude float NULL
Events Longitude float NULL
Events City nvarchar(100) NULL
Events MaxParticipants int NULL
Events MinParticipants int NULL
Events HoursEstimate decimal(5,2) NULL
Events SkillsRequired nvarchar(max) NULL
Events BenefitsDescription nvarchar(max) NULL
Events RewardPoints int NULL ((0))
Events IsVerified bit NULL ((0))
Events CreatedAt datetime NOT NULL (getdate())
Events UpdatedAt datetime NOT NULL (getdate())
Events PublishedAt datetime NULL
Events OrganizerId int NULL
EventSkills EventSkillId int NOT NULL
EventSkills Name nvarchar(50) NULL
EventSkills Difficulty int NULL
EventSkills ColorHex nvarchar(50) NULL
EventSkills BgColorHex nvarchar(50) NULL
EventTaskAssignments EventTaskAssignmentId int NOT NULL IDENTITY
EventTaskAssignments EventTaskId int NOT NULL
EventTaskAssignments StartTime datetime NOT NULL
EventTaskAssignments EndTime datetime NOT NULL
EventTaskAssignments Notes nvarchar(max) NULL
EventTaskAssignments Created datetime NOT NULL (getdate())
EventTaskAssignments Updated datetime NOT NULL (getdate())
EventTaskAssignments UserId int NULL
EventTasks EventTaskId int NOT NULL IDENTITY
EventTasks EventId int NOT NULL
EventTasks EventSkillId int NULL
EventTasks Name nvarchar(255) NOT NULL
EventTasks Description nvarchar(max) NULL
EventTasks Difficulty int NOT NULL ((1))
EventTasks Required bit NOT NULL ((1))
EventTasks MaxParticipants int NULL
EventTasks MinParticipants int NULL
EventTasks HoursEstimate decimal(5,2) NULL
EventTasks OrderIndex int NOT NULL ((0))
EventTasks CreatedAt datetime NOT NULL (getdate())
EventTasks UpdatedAt datetime NOT NULL (getdate())
EventTasks StartTime datetime NULL
EventTasks EndTime datetime NULL
EventTasks CreatedBy int NULL
EventTasks ModifiedBy int NULL
EventTypes EventTypeId int NOT NULL IDENTITY
EventTypes Name nvarchar(100) NOT NULL
EventTypes Description nvarchar(255) NULL
EventTypes Icon nvarchar(50) NULL
EventTypes ColorHex nvarchar(50) NULL
EventTypes BgColorHex nvarchar(50) NULL
EventTypes Created datetime NOT NULL (getdate())
GeneratorAsset AssetId int NOT NULL IDENTITY
GeneratorAsset FileName nvarchar(255) NOT NULL
GeneratorAsset ImagePng varbinary(max) NULL
GeneratorAsset ImageMetaDataJson nvarchar(max) NULL
GeneratorAsset Tags nvarchar(max) NULL
GeneratorAsset IsPublic bit NOT NULL ((0))
GeneratorAsset CreatedAt datetime2 NOT NULL (getutcdate())
GeneratorAsset UpdatedAt datetime2 NOT NULL (getutcdate())
GeneratorAsset DeletedAt datetime2 NULL
GeneratorAsset UserId int NULL
GeneratorLayer LayerId int NOT NULL IDENTITY
GeneratorLayer TemplateId int NOT NULL
GeneratorLayer AssetId int NULL
GeneratorLayer LayerType nvarchar(50) NOT NULL
GeneratorLayer Name nvarchar(255) NOT NULL
GeneratorLayer ZIndex int NOT NULL ((0))
GeneratorLayer SettingsJson nvarchar(max) NULL
GeneratorLayer CreatedAt datetime2 NOT NULL (getutcdate())
GeneratorLayer UpdatedAt datetime2 NOT NULL (getutcdate())
GeneratorRenderJob RenderJobId int NOT NULL IDENTITY
GeneratorRenderJob TemplateId int NOT NULL
GeneratorRenderJob Status nvarchar(50) NOT NULL ('Pending')
GeneratorRenderJob ImagePng varbinary(max) NULL
GeneratorRenderJob ErrorMessage nvarchar(max) NULL
GeneratorRenderJob StartedAt datetime2 NULL
GeneratorRenderJob CompletedAt datetime2 NULL
GeneratorRenderJob CreatedAt datetime2 NOT NULL (getutcdate())
GeneratorRenderJob UpdatedAt datetime2 NOT NULL (getutcdate())
GeneratorRenderJob UserId int NULL
GeneratorTemplate TemplateId int NOT NULL IDENTITY
GeneratorTemplate Name nvarchar(255) NOT NULL
GeneratorTemplate Description nvarchar(max) NULL
GeneratorTemplate Width int NOT NULL
GeneratorTemplate Height int NOT NULL
GeneratorTemplate IsPublished bit NOT NULL ((0))
GeneratorTemplate IsTemplate bit NOT NULL ((0))
GeneratorTemplate Tags nvarchar(max) NULL
GeneratorTemplate Category nvarchar(255) NULL
GeneratorTemplate CreatedAt datetime2 NOT NULL (getutcdate())
GeneratorTemplate UpdatedAt datetime2 NOT NULL (getutcdate())
GeneratorTemplate DeletedAt datetime2 NULL
GeneratorTemplate UserId int NULL
Groups GroupId int NOT NULL IDENTITY
Groups Name nvarchar(100) NOT NULL
Groups ColorHex nvarchar(50) NULL
Groups BgColorHex nvarchar(50) NULL
Groups IconType nvarchar(50) NULL
Groups Description nvarchar(255) NULL
Groups Publish bit NOT NULL ((0))
Groups Level int NOT NULL ((0))
Groups CreatedAt datetime NOT NULL (getutcdate())
Groups ModifiedAt datetime NOT NULL (getutcdate())
Invites InviteId int NOT NULL IDENTITY
Invites CreatedByTrackerId int NULL
Invites CreatedAt datetime NULL
Invites Name nvarchar(150) NULL
Invites Description nvarchar(500) NULL
Invites Notes nvarchar(max) NULL
Invites InviteType nvarchar(50) NULL
Invites ClaimedByTrackerId int NULL
Labels LabelId int NOT NULL IDENTITY
Labels Name nvarchar(100) NOT NULL
Labels ColorHex nvarchar(50) NULL
Labels BgColorHex nvarchar(50) NULL
Labels IconType nvarchar(50) NULL
Labels LabelType nvarchar(25) NULL
Labels Publish bit NOT NULL ((0))
Labels Level int NOT NULL ((0))
Locations LocationId bigint NOT NULL IDENTITY
Locations TrackerId int NOT NULL
Locations Latitude float NOT NULL
Locations Longitude float NOT NULL
Locations AccuracyMeters float NULL
Locations AltitudeMeters float NULL
Locations SpeedKmh float NULL
Locations HeadingDegrees float NULL
Locations Timestamp datetime NOT NULL (getutcdate())
Locations CreatedAt datetime NOT NULL (getutcdate())
Locations ModifiedAt datetime NOT NULL (getutcdate())
OrganizationRoles OrganizationRoleId int NOT NULL IDENTITY
OrganizationRoles Name nvarchar(50) NULL
OrganizationRoles ColorHex nvarchar(50) NULL
OrganizationRoles BgColorHex nvarchar(50) NULL
Organizations OrganizationId int NOT NULL IDENTITY
Organizations Name nvarchar(250) NULL
Organizations OwnedBy int NULL
Organizations CreatedBy int NULL
Organizations ModifiedBy int NULL
Organizations CreatedAt datetime NULL (getdate())
Organizations ModifiedAt datetime NULL (getdate())
OrganizationUsers OrganizationUserId int NOT NULL IDENTITY
OrganizationUsers UserId int NULL
OrganizationUsers OrganizationRoleId int NULL
OrganizationUsers AssignedBy int NULL
OrganizationUsers AssignedAt datetime NULL (getdate())
Roles RoleId int NOT NULL IDENTITY
Roles Name nvarchar(50) NULL
Roles HexColor nvarchar(50) NULL
Settings SettingId int NOT NULL IDENTITY
Settings SettingKey nvarchar(50) NOT NULL
Settings SettingValue nvarchar(100) NOT NULL
Settings Scope nvarchar(50) NULL
SocialPlatforms SocialPlatformId int NOT NULL IDENTITY
SocialPlatforms Name nvarchar(50) NULL
TrackerLabels TrackerLabelId int NOT NULL IDENTITY
TrackerLabels TrackerId int NOT NULL
TrackerLabels LabelId int NOT NULL
TrackerRoles TrackerRoleId int NOT NULL IDENTITY
TrackerRoles Name nvarchar(50) NOT NULL
TrackerRoles Description nvarchar(255) NULL
TrackerRoles CreatedAt datetime NOT NULL (getutcdate())
Trackers TrackerId int NOT NULL IDENTITY
Trackers GroupId int NULL
Trackers TrackerRoleId int NULL
Trackers DisplayName nvarchar(100) NOT NULL
Trackers Phone nvarchar(20) NULL
Trackers Email nvarchar(150) NULL
Trackers Mobile nvarchar(30) NULL
Trackers LicensePlate nvarchar(20) NULL
Trackers InviteCode uniqueidentifier NOT NULL (newid())
Trackers InviteName nvarchar(50) NULL
Trackers InvitedBy_TrackerId int NULL
Trackers QrGuid uniqueidentifier NOT NULL (newid())
Trackers JoinedAt datetime NOT NULL (getutcdate())
Trackers CreatedAt datetime NOT NULL (getutcdate())
Trackers ModifiedAt datetime NOT NULL (getutcdate())
Trackers CheckIn int NOT NULL ((0))
Trackers CheckInTime datetime NULL
Trackers CheckInLat float NULL
Trackers CheckInLon float NULL
Trackers CheckInBy_TrackerId int NULL
Trackers CheckInByTrackerId int NULL
Trackers InvitedByTrackerId int NULL
Trackers CityId int NULL
Trackers IsAdmin bit NULL
Trackers InviteId int NULL
Trackers UserId int NULL
UserGroups UserGroupId int NOT NULL IDENTITY
UserGroups UserId int NULL
UserGroups GroupId int NULL
UserLabels UserLabelId int NOT NULL IDENTITY
UserLabels UserId int NULL
UserLabels LabelId int NULL
UserRoles UserRoleId int NOT NULL IDENTITY
UserRoles UserId int NULL
UserRoles RoleId int NULL
Users FirstName nvarchar(100) NOT NULL
Users MiddleName nvarchar(100) NULL
Users LastName nvarchar(100) NOT NULL
Users Email nvarchar(255) NOT NULL
Users PasswordHash nvarchar(max) NOT NULL
Users CityId int NULL
Users MobilePhone nvarchar(20) NULL
Users ProfileImage varbinary(max) NULL
Users CoverImage varbinary(max) NULL
Users Bio nvarchar(max) NULL
Users Created datetime NOT NULL (getdate())
Users Modified datetime NOT NULL (getdate())
Users UserId int NOT NULL IDENTITY
Users Username nvarchar(100) NULL
UserSocialAccounts UserSocialAccountId int NOT NULL IDENTITY
UserSocialAccounts SocialPlatformId int NULL
UserSocialAccounts UserName nvarchar(255) NOT NULL
UserSocialAccounts Url nvarchar(max) NULL
UserSocialAccounts Verified bit NOT NULL ((0))
UserSocialAccounts Created datetime NOT NULL (getdate())
UserSocialAccounts Modified datetime NOT NULL (getdate())
UserSocialAccounts UserId int NULL
UserSocialAccounts ExternalId nvarchar(255) NULL

(248 rows affected)

```

### Row Counts
```
Table Row_Count
----- ---------
Cities 342
EventParticipants 0
Events 2
EventSkills 0
EventTaskAssignments 0
EventTasks 10
EventTypes 6
GeneratorAsset 0
GeneratorLayer 0
GeneratorRenderJob 0
GeneratorTemplate 0
Groups 7
Invites 2
Labels 6
Locations 1
OrganizationRoles 0
Organizations 1
OrganizationUsers 0
Roles 5
Settings 1
SocialPlatforms 8
TrackerLabels 0
TrackerRoles 3
Trackers 10
UserGroups 0
UserLabels 0
UserRoles 33
Users 33
UserSocialAccounts 2

(29 rows affected)

```

