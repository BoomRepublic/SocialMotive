# Admin Pages — AI Assistant Instructions

## Purpose
CRUD management pages for all core SocialMotive database tables. Only accessible to users with the `Admin` role.

## Pages in This Directory
| File | Route | Entity |
|------|-------|--------|
| `Index.razor` | `/admin` | Dashboard / navigation hub |
| `Users.razor` | `/admin/users` | `DbUser` (no popup use detail edit)|
| `User.razor` | `/admin/user/{id}` | `DbUser` (detail edit) |
| `Trackers.razor` | `/admin/trackers` | `DbTracker` (no popup use detail edit) |
| `Tracker.razor` | `/admin/tracker/{id}` | `DbTracker` (detail edit) |
| `TrackerRoles.razor` | `/admin/tracker-roles` | `DbTrackerRole` |
| `Events.razor` | `/admin/events` | `DbEvent` (no popup use detail edit) |
| `Event.razor` | `/admin/event/{id}` | `DbEvent` (detail edit) |
| `EventTypes.razor` | `/admin/event-types` | `DbEventType` (no popup use detail edit) |
| `EventType.razor` | `/admin/event-type/{id}` | `DbEventType` (detail edit) |
| `EventTasks.razor` | `/admin/event-tasks` | `DbEventTask` |
| `Groups.razor` | `/admin/groups` | `DbGroup` |
| `Invites.razor` | `/admin/invites` | `Invite` |
| `Labels.razor` | `/admin/labels` | `DbLabel` (no popup use detail edit) |
| `Label.razor` | `/admin/label/{id}` | `DbLabel` (detail edit) |
| `Locations.razor` | `/admin/locations` | `DbLocation` |
| `Cities.razor` | `/admin/cities` | `DbCity` |
| `Settings.razor` | `/admin/settings` | `DbSetting` |

## Overview Page Template
filename: {entity_plural}.razor
```razor
@page "/admin/{entity_pluralname}"
@layout MainLayout
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "Admin")]
@inject AdminApiService Api

<PageTitle>Admin - {Entity}</PageTitle>
```

## CRUD Page Template

filename: {entity_singular}.razor
```razor
@page "/admin/{entity}/{id}"
@layout MainLayout
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "Admin")]
@inject AdminApiService Api

<PageTitle>Admin - {Crud State} {Entity}</PageTitle>

## Conventions
- Use #telerik_ui_generator to generate ui
- Always include `@layout MainLayout` and `@attribute [Authorize(Roles = "Admin")]`
- Inject `AdminApiService` as `Api`
- Use Telerik components: `TelerikForm`, `TelerikDialog`, `TelerikNotification`
- one row per property with 2 columns (caption, edit control)
- Use `async/await` for all data calls in `OnInitializedAsync`
- Dropdowns must sort items alphabetically
- Use `<PageTitle>Admin - {Entity}</PageTitle>` pattern
- Wrap the page body in `<div class="admin-container">`
- Global `@using` statements are in `_Imports.razor` — add per-file only if truly page-specific
- Edit button in n-1 column
- Delete button in n column without caption, only icon
- Use TelerikColorPicker for all ColorHex and BgColorHex properties 
- If name, colorhex and bgcolor are available as properties create a badge in the name column using color and bgcolor.
