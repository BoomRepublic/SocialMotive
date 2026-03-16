# Auth Pages — AI Assistant Instructions

## Purpose
Authentication pages. These are publicly accessible (no `[Authorize]`) and use `@rendermode InteractiveServer`.

## Pages in This Directory
| File | Routes | Purpose |
|------|--------|---------|
| `Login.razor` | `/auth/login`, `/login` | Cookie-based sign-in |
| `Register.razor` | `/auth/register`, `/register` | New user registration |
| `ForgotPassword.razor` | `/auth/forgot-password` | Password reset request |

## Page Template
```razor
@page "/auth/{page}"
@rendermode InteractiveServer
@inject NavigationManager NavManager

<PageTitle>{Page} - SocialMotive</PageTitle>
```

## Conventions
- No `[Authorize]` attribute — these pages must be publicly accessible
- No `@layout MainLayout` — auth pages render standalone (default Blazor layout or none)
- Use `@rendermode InteractiveServer` for form interactivity
- Use Telerik components: `TelerikForm`, `TelerikTextBox`, `TelerikButton`
- Use Telerik CSS utility classes (`k-d-flex`, `k-justify-content-center`, `k-text-center`, etc.) for layout
- On successful login, redirect via `NavManager.NavigateTo("/admin")` or the return URL
- Auth API calls go to `/api/auth/` endpoints in `AuthController`
