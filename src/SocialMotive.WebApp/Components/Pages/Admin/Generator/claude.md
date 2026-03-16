# Admin/Generator Pages — AI Assistant Instructions

## Purpose
CRUD management pages for AssetGenerator entities. Accessible to users with the `Admin` or `AssetManager` role (policy: `"CanAccessGenerator"`).

## Pages in This Directory
| File | Route | Entity |
|------|-------|--------|
| `GeneratorTemplates.razor` | `/admin/generator/templates` | `DbTemplate` |
| `GeneratorAssets.razor` | `/admin/generator/assets` | `DbAsset` |
| `GeneratorLayers.razor` | `/admin/generator/layers` | `DbLayer` |
| `GeneratorRenderJobs.razor` | `/admin/generator/render-jobs` | `DbRenderJob` |

## Page Template
```razor
@page "/admin/generator/{entity}"
@layout MainLayout
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Policy = "CanAccessGenerator")]
@rendermode InteractiveServer
@inject GeneratorApiService Api

<PageTitle>Generator - {Entity}</PageTitle>
```

## Conventions
- Use `@attribute [Authorize(Policy = "CanAccessGenerator")]` (not role-based) so both `Admin` and `AssetManager` can access
- Inject `GeneratorApiService` as `Api` (named client `"GeneratorApi"` → `/api/generator/`)
- Use `@rendermode InteractiveServer`
- Use Telerik components for grids and forms
- DTOs live in `SocialMotive.Core.Model.Generator`
- Entity classes: `DbTemplate`, `DbAsset`, `DbLayer`, `DbRenderJob` in `SocialMotive.Core.Data.Generator`
