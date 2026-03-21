# SocialMotive.WebApp — AI Assistant Instructions

## Purpose
Blazor Web App host for SocialMotive. Serves Blazor components, registers DI, and loads API controllers from `SocialMotive.Core`. Contains no controllers or services of its own.

This app uses Interactive Server rendering (SSR). Razor component code executes on the server, not in the browser.

## Folder Structure
```
SocialMotive.WebApp/
  Components/
    Layout/               # MainLayout, AICodingAssistant sidebar
    Pages/
      Admin/              # CRUD pages for all core tables (Users, Trackers, Events, Labels, etc.)
        Generator/        # Generator pages (Templates, Assets, Layers, RenderJobs)
        Tools/            # Import utility page
      Auth/               # Login, Register, ForgotPassword
    App.razor             # Root component
    Routes.razor
    _Imports.razor        # Global @using statements for all Blazor components
  Properties/
    launchSettings.json
  wwwroot/                # Static assets (app.css, favicon, images)
  Program.cs              # Startup: DI, auth, middleware pipeline
  SocialMotive.WebApp.csproj
  appsettings.json
  appsettings.Development.json
```

## Key Architecture Points
- **Controllers live in `SocialMotive.Core/Controllers/`** — loaded into WebApp via `AddApplicationPart()` in Program.cs
- **Services live in `SocialMotive.Core/Services/`** — registered as scoped here in Program.cs
- WebApp is a pure Blazor host; all domain logic, data access, and API surfaces are in Core

## DI Registration (Program.cs)
Named HttpClients route to API prefixes:
| Named Client    | Base URL              |
|-----------------|-----------------------|
| `"AdminApi"`    | `/api/admin/`         |
| `"GeneratorApi"`| `/api/generator/`     |
| `"PublicApi"`   | `/api/`               |
| `"VolunteerApi"`| `/api/volunteer/`     |

Base URL is configured via `ApiBaseUrl` in appsettings (default: `https://localhost:7050`).

Services registered as scoped: `AdminApiService`, `GeneratorApiService`, `PublicApiService`, `VolunteerApiService`.

## Authentication
- Cookie auth, 8-hour sliding expiration, `HttpOnly`, `SameSite=Strict`, `SecurePolicy=Always`
- Login: `/login` | Logout: `/api/auth/demo-logout` | Access denied: `/access-denied`
- Authorization policies:
  - `"Admin"` — requires `Admin` role
  - `"CanAccessGenerator"` — requires `AssetManager` or `Admin`
  - `"CanAccessWeb"` — requires `Volunteer`, `Organizer`, or `Admin`

## Blazor Component Conventions
- All pages use `@layout MainLayout`
- Protected pages declare `@attribute [Authorize(Roles = "...")]`
- Components inject services via `@inject AdminApiService Api` (or the relevant service)
- Global usings are in `_Imports.razor` — add new project-wide usings there, not per-file
- Interactive render mode: `AddInteractiveServerComponents()` / `AddInteractiveServerRenderMode()`
- Do **not** create a C# SignalR `HubConnection` inside a Razor component to reach back into the app from the server side.
- If a page needs browser realtime updates, create the SignalR connection in browser JavaScript and treat the Razor component as SSR + initial state.

## Adding a New Admin Page
1. Create `Components/Pages/Admin/MyEntity.razor` with `@page "/admin/my-entity"`
2. Inject the appropriate API service
3. Use Telerik components (`TelerikGrid`, `TelerikForm`, etc.) for UI
4. No need to touch Program.cs unless a new service or HttpClient is needed
