# Admin/Tools Pages — AI Assistant Instructions

## Purpose
Utility and maintenance pages for the Admin section. Only accessible to users with the `Admin` role.

## Pages in This Directory
| File | Route | Purpose |
|------|-------|---------|
| `Import.razor` | `/admin/tools/import` | Bulk JSON/CSV import into any database table |

## Page Template
```razor
@page "/admin/tools/{tool}"
@layout MainLayout
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "Admin")]
@inject AdminApiService Api
@inject NavigationManager Navigation

<PageTitle>Admin - {Tool}</PageTitle>
```

## Conventions
- Always include `@layout MainLayout` and `@attribute [Authorize(Roles = "Admin")]`
- Tools pages may use `HttpClient` directly for file upload/streaming scenarios
- New tools should be added to the Admin Index dashboard nav links
