# SocialMotive.Core — AI Assistant Instructions

## Purpose
Shared class library containing domain models, DTOs, DbContext, API services, and Generator types. Referenced by `SocialMotive.WebApp`.

## Folder Structure
```
SocialMotive.Core/
  Authorization/            # AppRoles constants
  Controllers/              # Shared API controllers (e.g. GeneratorController)
  Data/                     # EF Core entity classes (Db-prefixed), DbContext
    Generator/              # Generator-specific entities (DbTemplate, DbAsset, DbLayer, DbRenderJob)
    SocialMotiveDbContext.cs
  Generator/                # Generator utility types (settings, transforms, metadata)
  Mapping/                  # AutoMapper profiles (AdminMappingProfile, GeneratorMappingProfile, etc.)
  Model/
    Admin/                  # DTOs for Admin API
    Generator/              # DTOs for Generator API
    Public/                 # DTOs for Public API
    Volunteer/              # DTOs for Volunteer API
  Services/                 # API client services used by Blazor components
```

## Entity Classes (Data/)
All entity classes are prefixed with `Db` to distinguish them from DTOs:
- `DbUser`, `DbTracker`, `DbEvent`, `DbEventType`, `DbEventTask`, `DbEventParticipant`, `DbEventTaskAssignment`
- `DbGroup`, `DbLabel`, `DbTrackerLabel`, `DbTrackerRole`
- `DbCity`, `DbLocation`, `DbInvite`, `DbSetting`, `DbUserSocialAccount`
- Generator: `DbTemplate`, `DbAsset`, `DbLayer`, `DbRenderJob`

File names match class names (e.g., `DbUser.cs` contains `class DbUser`).

## Naming Conventions
- **Entity classes**: `Db` prefix (e.g., `DbUser`), one class per file, file named after class
- **DTOs**: No prefix, no suffix (e.g., `User`, `Event`)
- **Namespace**: `SocialMotive.Core.Data` for entities, `SocialMotive.Core.Data.Generator` for generator entities
- **Primary keys**: `{SingularTableName}Id` (e.g., `UserId`, `TrackerId`, `TemplateId`)

## DbContext
- Single canonical `SocialMotiveDbContext` in `Data/SocialMotiveDbContext.cs`
- DbSet property names are plural (e.g., `DbSet<DbUser> Users`)
- Fluent API for all relationships, constraints, indexes, and table mappings
- Generator tables mapped with `ToTable("GeneratorTemplate")`, etc.
- FK delete behaviors explicitly configured

## DTOs
- DTOs do NOT use the `Db` prefix
- DTOs do NOT use the `Dto` suffix
- Located in `Model/{Area}/` (Admin, Generator, Public, Volunteer)
- **Namespaces**: `SocialMotive.Core.Model.Admin`, `SocialMotive.Core.Model.Generator`, `SocialMotive.Core.Model.Public`, `SocialMotive.Core.Model.Volunteer`

## Services (Services/)
- One service class per feature area: `AdminApiService`, `GeneratorApiService`, `PublicApiService`, `VolunteerApiService`
- **Namespace**: `SocialMotive.Core.Services`
- Use `IHttpClientFactory` with named clients (`"AdminApi"`, `"GeneratorApi"`, `"PublicApi"`, `"VolunteerApi"`)
- Forward auth cookies via `IHttpContextAccessor` (except `PublicApiService`)
- Return `null` or empty collections on HTTP failure — no exceptions propagated to UI

## Other Types
- `Generator/` — `GeneratorSettings`, `LayerSettings`, `Transform`, `ImageMetaData`, etc.
- `Authorization/AppRoles.cs` — role name constants
- `Mapping/` — AutoMapper profiles (`AdminMappingProfile`, `GeneratorMappingProfile`, `PublicMappingProfile`, `VolunteerMappingProfile`)
