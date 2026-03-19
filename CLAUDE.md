# SocialMotive - AI Assistant Instructions

## Project Overview
SocialMotive is a worker/volunteer/event organizer platform built as a unified Blazor Web App. It has these feature areas:
- **Public** — Public anonymous
- **TelegramBot** — Telegram bot that interacts with platform user
- **User** — Logged in user, access to generator and telegrambot
- **Worker** — Worker (paid) portal (browse events, RSVP, track participation)
- **Volunteer** — Volunteer portal (browse events, RSVP, track participation)
- **Organizer** — Organizer portal (organize events, create tasks, track participation, manage budget)
- **Generator** — AssetGenerator canvas editor for promotional images
- **Admin** — CRUD data management dashboard

**MVP target: March 31, 2026**

## Tech Stack
- **.NET 10.0** (SDK 10.0.0)
- **Blazor Web App** — Interactive Server render mode
- **Telerik UI for Blazor 13.0.0** — primary UI component library
- **Entity Framework Core 10.0.0** with **SQL Server** **Code First Approach**
- **Cookie-based claims authentication** with role-based authorization
- **Swagger/Swashbuckle** for API documentation
- **Telerik Document Processing** for image/canvas operations in the Generator
- **Telegram.Bot 22.9.5.3** — Telegram bot integration for live location tracking & account linking

## Solution Structure
```
src/
  SocialMotive.Core/       # Shared class library: Models, DTOs, DbContext, Generator types
  SocialMotive.WebApp/     # Blazor Web App: Components, Controllers, Services, wwwroot
```

- `SocialMotive.Core` is the shared library containing domain models, DTOs, and the canonical `SocialMotiveDbContext`
- `SocialMotive.WebApp` is the Blazor host with API controllers, Blazor components, and services

## Architecture & Data Flow
```
Blazor Component → ApiService (HttpClient) → Controller → DbContext → SQL Server
```

- API controllers live in `Controllers/` and are attribute-routed under `api/`
- Blazor components call the API through service classes in `Services/` using `IHttpClientFactory` (named clients: `"AdminApi"`, `"GeneratorApi"`, `"PublicApi"`, `"VolunteerApi"`, `"TelegramApi"`)
- Services forward auth cookies from the incoming HTTP context to outgoing API calls
- Controllers inject `SocialMotiveDbContext` directly (no repository/service abstraction layer)
- **Telegram Bot**: `TelegramBotService` (BackgroundService, long-polling) delegates updates to `TelegramUpdateHandler` (singleton). Link codes use in-memory `ConcurrentDictionary` with expiry.

## Authorization
- Cookie auth with 8-hour sliding expiration
- Roles: Get roles from database
- Controllers use `[Authorize(Roles = "...")]`
- Blazor components receive cascading `AuthenticationState`

## Coding Conventions
- **One class per file** — every class, record, and interface gets its own file
- **Enums** — grouped in a single `Enums.cs` file per namespace/folder
- **C# nullable reference types** enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** enabled
- **AutoMapper 16.1.1** — DTO ↔ Entity mapping via Profile classes in `SocialMotive.Core/Mapping/`; controllers inject `IMapper`; use `ProjectTo<T>()` for queries, `_mapper.Map<T>()` for single objects, `_mapper.Map(source, dest)` for updates; system-managed fields (timestamps, passwords) are set manually after mapping
- **`#region` blocks** to organize code by entity/feature within files
- **XML doc comments** on controller actions
- **`async/await`** throughout all data access and API calls
- **Error handling**: try/catch in controller actions returning `BadRequest(new { message, error })` with anonymous objects
- **Naming**: PascalCase for public members, camelCase for local variables, `I`-prefix for interfaces
- **Entity class naming**: All EF Core entity/model classes in `SocialMotive.Core.Data` are prefixed with `Db` (e.g., `DbUser`, `DbEvent`, `DbTemplate`). DTOs do NOT use the prefix or the `Dto` suffix.
- **Dropdowns** — sort items alphabetically

## Database
- database: `socialmotive`
- accessed through a single `SocialMotiveDbContext` in `SocialMotive.Core`
- Generator tables use explicit `ToTable(...)` mappings
- Fluent API configuration for relationships, constraints, and indexes
- FK delete behaviors explicitly configured (no cascade defaults)

## Controller Patterns
- `[ApiController]` with `[Route("api/{area}")]`
- Standard CRUD per entity: `GET /` (list), `GET /{id}` (by id), `POST /` (create), `PUT /{id}` (update), `DELETE /{id}` (delete)
- Generic `[HttpPost("import")]` endpoint for JSON bulk import
- Return `Ok(entity)` on success, `NotFound()` or `BadRequest(...)` on failure

## Service Patterns (ApiService)
- One service class per feature area (e.g., `AdminApiService`)
- Registered as scoped in DI
- Uses `IHttpClientFactory` with named client
- Returns `null` or empty collections on HTTP failure (no exceptions to UI)
- Mirrors controller endpoint structure method-for-method

## Known Issues / Tech Debt
- Duplicate `SocialMotiveDbContext` exists in both `SocialMotive.Core` and `SocialMotive.WebApp.Data` — the Core version is canonical; the WebApp copy may be stale
- `ImportRequest`/`ImportResponse`/`ImportResult` DTOs are duplicated in both the controller and the service files — should be consolidated into `SocialMotive.Core`
- Generator table naming inconsistency between the two DbContext copies (singular vs. plural)

## File Locations
- **Models**: `src/SocialMotive.Core/Model/`
- **DTOs**: `src/SocialMotive.Core/Model/`
- **Telegram DTOs**: `src/SocialMotive.Core/Model/Telegram/`
- **Mapping Profiles**: `src/SocialMotive.Core/Mapping/`
- **DbContext**: `src/SocialMotive.Core/Data/SocialMotiveDbContext.cs`
- **Controllers**: `src/SocialMotive.WebApp/Controllers/`
- **Telegram Controller**: `src/SocialMotive.Core/Controllers/TelegramController.cs`
- **Services**: `src/SocialMotive.WebApp/Services/`
- **Telegram Bot Services**: `src/SocialMotive.Core/Services/Telegram/`
- **Blazor Components**: `src/SocialMotive.WebApp/Components/`
- **Blazor Pages**: `src/SocialMotive.WebApp/Components/Pages/`
- **Layout**: `src/SocialMotive.WebApp/Components/Layout/`
- **SQL Scripts**: `sql/`
- **Planning**: `planning/`
- **Diagrams**: `diagrams/`
