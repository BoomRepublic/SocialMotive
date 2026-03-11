# SocialMotive Platform - Technical Design

## 1. Technology Stack

### 1.1 Frontend
- **UI Framework**: Blazor Web App (Auto/Interactive rendering)
- **UI Components**: Telerik UI for Blazor
- **Language**: C# 12 / .NET 10.0
- **Build Tool**: .NET CLI / MSBuild
- **State Management**: Blazor cascading parameters + local component state
- **HTTP Client**: HttpClient with typed services
- **Canvas/Graphics**: HTML5 Canvas API (JavaScript interop) for TrekkerGenerator editor

### 1.2 Backend API
- **Runtime**: .NET 10.0 ASP.NET Core
- **API Pattern**: REST (JSON over HTTPS)
- **Controllers**: MVC attribute-routed controllers (no minimal endpoints in v1)
- **ORM**: Entity Framework Core 10.x
- **Database**: SQL Server (2017+, MSSQL17 confirmed)
- **Authentication**: ASP.NET Core Authentication + OIDC/OAuth2 integration
- **Authorization**: Claims-based policies

### 1.3 Data Layer
- **Database**: Microsoft SQL Server
- **Schema Management**: Entity Framework Core Migrations
- **Connection Pooling**: Built-in (configurable pool size 10-100)
- **Query Optimization**: Indexed columns on FK, search, and sort fields

### 1.4 Supporting Libraries
- **Validation**: FluentValidation or DataAnnotations
- **Mapping**: Mapster or AutoMapper
- **Logging**: Serilog (console + file sinks)
- **Serialization**: System.Text.Json (default ASP.NET Core)
- **Async Utilities**: standard async/await patterns
- **Testing**: xUnit + Moq (for unit tests)

### 1.5 Deployment Infrastructure
- **Hosting**: Azure App Service or IIS (on-premises)
- **Load Balancer**: Azure Load Balancer or hardware LB
- **SSL/TLS**: Let's Encrypt or purchased certificates
- **DNS**: Azure DNS or registrar
- **Storage**: Azure Blob Storage (for assets) OR local filesystem + backup
- **Monitoring**: Application Insights or ELK stack
- **CI/CD**: GitHub Actions or Azure Pipelines

---

## 2. Project Structure

### 2.1 Solution Layout
```
SocialMotive/
├── src/
│   ├── SocialMotive.Core/                  # Shared contracts & base classes
│   │   ├── Api/
│   │   │   ├── Abstractions/               # IApiService, ApiServiceBase
│   │   │   ├── Contracts/                  # ApiEnvelopeDto, ApiErrorDto, etc.
│   │   │   └── Models/                     # Helper models
│   │   ├── Frontend/
│   │   │   ├── Abstractions/               # IDashboardApiService, ILookupApiService
│   │   │   └── Contracts/                  # EventDto, UserProfileDto, etc.
│   │   ├── TrekkerGenerator/
│   │   │   ├── Abstractions/               # ITemplatesApiService, etc.
│   │   │   └── Contracts/                  # CanvasDocumentDto, LayerDto, etc.
│   │   ├── Admin/
│   │   │   ├── Abstractions/               # IMetadataApiService, ITableCrudApiService
│   │   │   └── Contracts/                  # TableDescriptorDto, GridDataRequestDto, etc.
│   │   └── Auth/
│   │       ├── Claims.cs                   # Claim constants
│   │       ├── AuthExtensions.cs           # OIDC middleware setup
│   │       └── Policies.cs                 # Policy definitions
│   │
│   ├── SocialMotive.TrekkerGenerator.Core/ # Generator business logic
│   │   ├── Domain/
│   │   │   ├── Entities/                   # Template, Asset, Layer, RenderJob, User
│   │   │   ├── ValueObjects/               # CanvasSize, LayerPosition, etc.
│   │   │   └── Exceptions/                 # DomainException, InvalidCanvasException
│   │   ├── Application/
│   │   │   ├── Services/
│   │   │   │   ├── ITemplateService.cs
│   │   │   │   ├── IAssetService.cs
│   │   │   │   ├── ICanvasRenderService.cs
│   │   │   │   └── IAiImageService.cs      # Provider-agnostic
│   │   │   ├── DTOs/                       # Copy of contract DTOs (keep in sync)
│   │   │   └── Handlers/                   # Use-case command handlers (optional, if CQRS later)
│   │   └── Infrastructure/
│   │       ├── Persistence/
│   │       │   ├── TrekkerGeneratorDbContext.cs
│   │       │   ├── Migrations/
│   │       │   └── Repositories/           # ITemplateRepository, IAssetRepository
│   │       ├── Storage/
│   │       │   ├── FileStorageService.cs   # Local FS or Azure Blob
│   │       │   └── IStorageService.cs
│   │       └── Rendering/
│   │           └── PngRenderService.cs     # Canvas → PNG (headless approach or SkiaSharp)
│   │
│   ├── SocialMotive.Frontend.Web/          # Volunteer portal
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   ├── Components/
│   │   │   ├── Layout/
│   │   │   │   ├── MainLayout.razor
│   │   │   │   └── NavMenu.razor
│   │   │   ├── Pages/
│   │   │   │   ├── Dashboard.razor         # Event list
│   │   │   │   ├── EventDetail.razor
│   │   │   │   ├── MyEvents.razor
│   │   │   │   ├── Profile.razor
│   │   │   │   └── Index.razor             # Home/redirect
│   │   │   └── Shared/
│   │   │       ├── EventCard.razor
│   │   │       ├── UserProfile.razor
│   │   │       └── NotificationToast.razor
│   │   ├── Controllers/
│   │   │   ├── DashboardController.cs      # GET /api/dashboard/*
│   │   │   └── LookupController.cs         # GET /api/lookups
│   │   ├── Services/
│   │   │   ├── Api/
│   │   │   │   ├── DashboardApiService.cs  # Implements IDashboardApiService
│   │   │   │   └── LookupApiService.cs
│   │   │   └── Auth/
│   │   │       └── FrontendAuthService.cs  # User context, claims mapping
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   │   ├── app.css
│   │   │   │   └── bootstrap.css
│   │   │   └── js/
│   │   │       └── app.js
│   │   └── appsettings.json / appsettings.Development.json
│   │
│   ├── SocialMotive.TrekkerGenerator.Web/  # Canvas editor + promo tool
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   ├── Components/
│   │   │   ├── Layout/
│   │   │   │   └── MainLayout.razor
│   │   │   ├── Pages/
│   │   │   │   ├── Index.razor
│   │   │   │   ├── Editor.razor            # Canvas editor page
│   │   │   │   └── Templates.razor
│   │   │   └── Shared/
│   │   │       ├── CanvasRenderer.razor    # Wraps JS canvas
│   │   │       ├── LayerPanel.razor
│   │   │       └── StyleControls.razor
│   │   ├── Controllers/
│   │   │   ├── TemplatesController.cs      # CRUD /api/templates
│   │   │   ├── AssetsController.cs         # Upload /api/assets
│   │   │   ├── RenderController.cs         # Export /api/render
│   │   │   └── AiController.cs             # /api/ai (v2+)
│   │   ├── Services/
│   │   │   └── Api/
│   │   │       ├── TemplatesApiService.cs
│   │   │       ├── AssetsApiService.cs
│   │   │       ├── RenderApiService.cs
│   │   │       └── AiApiService.cs
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   │   └── editor.css
│   │   │   └── js/
│   │   │       ├── canvas-editor.js        # Canvas manipulation, layer mgmt
│   │   │       └── interop.js              # Blazor ↔ JS interop
│   │   ├── appsettings.json
│   │   └── appsettings.Production.json
│   │
│   └── SocialMotive.AdminBackend.Web/      # Admin CRUD & moderation
│       ├── Program.cs
│       ├── App.razor
│       ├── Components/
│       │   ├── Layout/
│       │   │   └── MainLayout.razor
│       │   ├── Pages/
│       │   │   ├── Index.razor
│       │   │   ├── TableBrowser.razor      # List of tables
│       │   │   ├── TableCrud.razor         # Telerik grid CRUD
│       │   │   ├── AuditLog.razor          # (v2+)
│       │   │   └── Export.razor            # (v2+)
│       │   └── Shared/
│       │       ├── GridToolbar.razor       # Add, Delete, Export buttons
│       │       └── FilterPanel.razor
│       ├── Controllers/
│       │   ├── MetadataController.cs       # Table/column metadata
│       │   ├── TableCrudController.cs      # Generic CRUD per table
│       │   └── ExportController.cs         # CSV/Excel export
│       ├── Services/
│       │   └── Api/
│       │       ├── MetadataApiService.cs
│       │       ├── TableCrudApiService.cs
│       │       └── ExportApiService.cs
│       ├── Admin/
│       │   ├── TableWhitelist.cs           # Allowed tables
│       │   ├── DynamicGridBuilder.cs       # Metadata → Telerik grid
│       │   └── AdminAuthorizationService.cs
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── admin.css
│       │   └── js/
│       │       └── grid-helpers.js
│       └── appsettings.json
│
├── tests/
│   ├── SocialMotive.TrekkerGenerator.Web.Tests/
│   │   ├── Unit/
│   │   │   ├── Services/
│   │   │   │   ├── TemplateServiceTests.cs
│   │   │   │   └── CanvasRenderServiceTests.cs
│   │   │   └── Controllers/
│   │   │       └── TemplatesControllerTests.cs
│   │   ├── Integration/
│   │   │   ├── ApiTests.cs
│   │   │   └── DatabaseTests.cs
│   │   ├── Fixtures/
│   │   │   └── TestDbContextFactory.cs
│   │   └── appsettings.Test.json
│   │
│   └── SocialMotive.Frontend.Web.Tests/     # (minimal, mainly API service tests)
│       └── Services/
│           └── DashboardApiServiceTests.cs
│
├── SQL/
│   ├── socialmotive-create-v1.sql           # Existing SocialMotive DB
│   └── trekkergenerator-create-v1.sql       # New TrekkerGenerator DB
│
├── planning/
│   ├── plan-socialMotiveTrekkerGenerator.prompt.md
│   └── functional-design.md
│
└── SocialMotive.sln                         # Master solution file
```

---

## 3. Database Design

### 3.1 SocialMotive Database (Existing)
- Tables: Users, Trackers, Events, EventTypes, EventTasks, EventParticipants, Groups, Labels, Cities, Locations, Settings, etc.
- New column in Users: `ExternalSubjectId` (nullable, NVARCHAR(500), UNIQUE)
- Optional table: AuditLogs (for admin mutations, v1 MVP optional)

### 3.2 TrekkerGenerator Database
- **Users table**:
  ```sql
  CREATE TABLE [dbo].[Users] (
    [UserId] [uniqueidentifier] PRIMARY KEY DEFAULT NEWID(),
    [ExternalSubjectId] [nvarchar](500) UNIQUE,
    [Email] [nvarchar](255) NOT NULL,
    [DisplayName] [nvarchar](100),
    [CreatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [LastSeenUtc] [datetime],
    [rowversion] [timestamp]
  );
  ```

- **Templates table**:
  ```sql
  CREATE TABLE [dbo].[Templates] (
    [TemplateId] [uniqueidentifier] PRIMARY KEY DEFAULT NEWID(),
    [CreatedByUserId] [uniqueidentifier] NOT NULL FOREIGN KEY (Users.UserId),
    [Name] [nvarchar](100) NOT NULL,
    [CanvasWidth] [int] NOT NULL,
    [CanvasHeight] [int] NOT NULL,
    [CanvasJson] [nvarchar](max), -- Serialized CanvasDocumentDto
    [CreatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [UpdatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [rowversion] [timestamp],
    INDEX IX_Templates_UserId (CreatedByUserId),
    INDEX IX_Templates_UpdatedUtc (UpdatedUtc DESC)
  );
  ```

- **Assets table**:
  ```sql
  CREATE TABLE [dbo].[Assets] (
    [AssetId] [uniqueidentifier] PRIMARY KEY DEFAULT NEWID(),
    [UploadedByUserId] [uniqueidentifier] NOT NULL FOREIGN KEY (Users.UserId),
    [FileName] [nvarchar](255) NOT NULL,
    [ContentType] [nvarchar](50), -- e.g. 'image/png', 'image/jpeg'
    [StoragePath] [nvarchar](1000) NOT NULL, -- local path or blob URL
    [FileSizeBytes] [bigint],
    [ImageWidthPx] [int],
    [ImageHeightPx] [int],
    [FileHash] [nvarchar](64), -- SHA256 for dedup
    [CreatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [ExpiresUtc] [datetime], -- optional cleanup window
    [rowversion] [timestamp],
    INDEX IX_Assets_UserId (UploadedByUserId),
    INDEX IX_Assets_CreatedUtc (CreatedUtc DESC)
  );
  ```

- **Layers table** (flat, not normalized, for query flexibility):
  ```sql
  CREATE TABLE [dbo].[Layers] (
    [LayerId] [uniqueidentifier] PRIMARY KEY DEFAULT NEWID(),
    [TemplateId] [uniqueidentifier] NOT NULL FOREIGN KEY (Templates.TemplateId) ON DELETE CASCADE,
    [LayerType] [nvarchar](50) NOT NULL CHECK (LayerType IN ('Text', 'Image')), -- enum
    [Name] [nvarchar](100),
    [PositionX] [int] NOT NULL,
    [PositionY] [int] NOT NULL,
    [Width] [int] NOT NULL,
    [Height] [int] NOT NULL,
    [Rotation] [float] DEFAULT 0,
    [Opacity] [float] DEFAULT 1.0 CHECK (Opacity >= 0 AND Opacity <= 1),
    [ZIndex] [int] NOT NULL,
    [IsVisible] [bit] DEFAULT 1,
    -- Text layer fields
    [Text] [nvarchar](500),
    [FontFamily] [nvarchar](100)],
    [FontSizePixels] [int],
    [FontWeight] [nvarchar](50)], -- bold, normal, light
    [TextColor] [nvarchar](10)], -- #RRGGBB or #RRGGBBAA
    [TextAlignment] [nvarchar](50)], -- left, center, right
    [LineHeight] [float],
    -- Image layer fields
    [AssetId] [uniqueidentifier] FOREIGN KEY (Assets.AssetId),
    [CreatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [UpdatedUtc] [datetime] DEFAULT GETUTCDATE(),
    [rowversion] [timestamp],
    INDEX IX_Layers_TemplateId (TemplateId, ZIndex)
  );
  ```

- **RenderJobs table** (tracks export history):
  ```sql
  CREATE TABLE [dbo].[RenderJobs] (
    [RenderJobId] [uniqueidentifier] PRIMARY KEY DEFAULT NEWID(),
    [TemplateId] [uniqueidentifier] NOT NULL FOREIGN KEY (Templates.TemplateId),
    [RequestedByUserId] [uniqueidentifier] NOT NULL FOREIGN KEY (Users.UserId),
    [Status] [nvarchar](50) NOT NULL CHECK (Status IN ('Pending', 'Running', 'Succeeded', 'Failed')) DEFAULT 'Pending',
    [ExportWidth] [int],
    [ExportHeight] [int],
    [TransparentBackground] [bit] DEFAULT 0,
    [OutputAssetId] [uniqueidentifier] FOREIGN KEY (Assets.AssetId),
    [ErrorMessage] [nvarchar](max),
    [RequestedUtc] [datetime] DEFAULT GETUTCDATE(),
    [CompletedUtc] [datetime],
    [rowversion] [timestamp],
    INDEX IX_RenderJobs_TemplateId (TemplateId),
    INDEX IX_RenderJobs_Status_RequestedUtc (Status, RequestedUtc DESC)
  );
  ```

### 3.3 Indexing Strategy
- **FK columns**: Create non-clustered index (e.g., `IX_Templates_UserId`)
- **Search/sort columns**: `UpdatedUtc DESC`, `CreatedUtc DESC`
- **Status/state filtering**: Index on status enum (e.g., `IX_RenderJobs_Status`)
- **Composite indexes** for paging: (`UserId` ASC, `CreatedUtc` DESC)

---

## 4. API Design Patterns

### 4.1 REST Convention
- **Naming**: Plural resources (`/api/events`, `/api/templates`, `/api/assets`)
- **Methods**:
  - GET (read, safe, idempotent)
  - POST (create, idempotent key for duplicate safety)
  - PUT (update full resource)
  - PATCH (partial update, v2+)
  - DELETE (logical delete with audit, or hard delete)
- **Status Codes**:
  - 200 OK (success)
  - 201 Created (resource created)
  - 204 No Content (delete success)
  - 400 Bad Request (validation error)
  - 401 Unauthorized (missing/invalid token)
  - 403 Forbidden (insufficient permissions)
  - 404 Not Found
  - 409 Conflict (concurrency, duplicate key)
  - 500 Internal Server Error

### 4.2 Response Envelope (Recommended v1+)
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string[] Errors { get; set; }
    public string CorrelationId { get; set; } // For tracing
    public DateTime Timestamp { get; set; }
}
```

**Usage:**
```json
{
  "success": true,
  "data": { "eventId": "uuid", "title": "Cleanup Day" },
  "errors": null,
  "correlationId": "req-12345",
  "timestamp": "2026-03-11T10:00:00Z"
}
```

### 4.3 Pagination
```csharp
public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string SortBy { get; set; } = "CreatedUtc desc";
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

**Query string**: `GET /api/events?page=2&pageSize=10&sortBy=StartDate%20asc`

### 4.4 Error Response
```json
{
  "success": false,
  "data": null,
  "errors": {
    "email": ["Email is required", "Email format invalid"],
    "name": ["Name is required"]
  },
  "correlationId": "req-12345",
  "timestamp": "2026-03-11T10:00:00Z"
}
```

---

## 5. Authentication & Authorization

### 5.1 OIDC/OAuth2 Setup
- **Provider**: Configured in appsettings (e.g., Azure AD, Auth0)
- **Middleware**: `AddAuthentication("OpenIdConnect")`
- **Redirect URIs** (per app):
  - `https://socialmotive.net/signin-oidc`
  - `https://trekkergenerator.socialmotive.net/signin-oidc`
  - `https://admin.socialmotive.net/signin-oidc`
- **Post-logout URIs**:
  - `https://socialmotive.net`
  - `https://trekkergenerator.socialmotive.net`
  - `https://admin.socialmotive.net`

### 5.2 Token-to-User Sync
1. User hits `/signin-oidc` callback
2. OnTokenValidated event → extract `sub` (subject ID)
3. Upsert Users table: `ExternalSubjectId = sub`
4. Set ClaimsPrincipal with roles (from JWT or DB)

### 5.3 Claims & Policies
```csharp
// Claims
public static class ClaimTypes
{
    public const string Role = "role";
    public const string Email = "email";
    public const string Subject = "sub";
    public const string AdminLevel = "admin_level"; // Custom
}

// Policies
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
    
    options.AddPolicy("OrganizerOrAdmin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Organizer", "Admin"));
});
```

### 5.4 API Authorization
- **Frontend**: All endpoints require `[Authorize]` (authenticated user)
- **TrekkerGenerator**: All endpoints require `[Authorize("OrganizerOrAdmin")]`
- **Admin**: All endpoints require `[Authorize("AdminOnly")]`

---

## 6. Deployment Architecture

### 6.1 Staging & Production
```
DNS
├── socialmotive.net          → Frontend.Web App Service
├── trekkergenerator.socialmotive.net → TrekkerGenerator.Web App Service
└── admin.socialmotive.net    → AdminBackend.Web App Service

App Services (per region, e.g., West Europe)
├── SocialMotive-Frontend-AppService (slots: staging, production)
├── SocialMotive-TrekkerGen-AppService (slots: staging, production)
└── SocialMotive-Admin-AppService (slots: staging, production)

Databases
├── SocialMotive logical server
│   └── socialmotive DB (read replicas in v2+)
└── TrekkerGenerator logical server
    └── trekkergenerator DB
```

### 6.2 Deployment Pipeline
1. **Build**: dotnet restore → dotnet build → dotnet test
2. **Package**: dotnet publish → ZIP artifact
3. **Deploy to Staging**: App Service slot
4. **Smoke Tests**: Health checks, key APIs
5. **Manual Validation** (or auto if tests pass)
6. **Swap to Production**: Zero-downtime slot swap
7. **Rollback Plan**: Keep previous version in staging for quick rollback

### 6.3 Scaling Considerations
- **Horizontal Scaling**: App Service auto-scale rules (CPU > 70%, scale out)
- **Database**: Connection pool size 50-100 (Azure SQL auto-adjusts)
- **Storage**: Blob storage with CDN for asset downloads (v2+)
- **Caching**: Redis for session/template cache (v2+)

---

## 7. Data Security & Compliance

### 7.1 Encryption
- **In Transit**: HTTPS only (TLS 1.2+)
- **At Rest**:
  - SQL Server: Transparent Data Encryption (TDE) enable
  - Blob Storage: Server-side encryption (default in Azure)
  - User files: encryption key per customer (v2+)

### 7.2 PII Handling
- **User profiles**: Encrypted at rest in DB (optional, v2+)
- **Audit logs**: Always log, encrypt sensitive fields
- **File upload**: Virus scan (ClamAV, v2+)
- **GDPR**: Export user data endpoint (v2+), right-to-delete (soft delete)

### 7.3 Rate Limiting & DDoS
- **API Rate Limit** (v2+): 100 req/min per user (standard), 1000 for premium
- **DDoS Protection**: Azure DDoS Protection Standard (v2+)

---

## 8. Monitoring & Logging

### 8.1 Logging (Serilog)
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(new TelemetryConverter())
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", env.EnvironmentName)
    .CreateLogger();
```

### 8.2 Structured Logging
- Every API call: `LogInformation("Processing {MethodName} request", nameof(GetEvents))`
- Errors: `LogError(ex, "Failed to render canvas {CanvasId}", canvasId)`
- Performance: `LogInformation("Database query took {ElapsedMs}ms", watch.ElapsedMilliseconds)`

### 8.3 Application Insights (optional, best practice)
- Track dependencies (SQL, HTTP calls)
- Custom metrics (render time, upload size)
- User sessions + flows
- Alerts on error rate > 5%, response time > 2sec

---

## 9. Testing Strategy

### 9.1 Unit Tests
- **DTO Mappings**: Verify Mapster/AutoMapper
- **Validation Rules**: FluentValidation test cases
- **Business Logic**: Service methods in isolation (mocked repos)

### 9.2 Integration Tests
- **Database**: Use TestDbContext (InMemory or LocalDB in tests)
- **API Controllers**: HttpClient calls to TestServer
- **End-to-End**: Full request/response cycle (POST /api/templates → verify DB insert)

### 9.3 Test Coverage Target
- **MIN v1**: 60% (critical services, controllers)
- **v2+**: 80% (all public APIs, important use-cases)

### 9.4 Test Framework
```csharp
[Fact]
public async Task CreateTemplate_WithValidInput_ReturnsCreatedTemplate()
{
    // Arrange
    var request = new CreateTemplateRequestDto { Name = "Test" };
    
    // Act
    var result = await _service.CreateAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

---

## 10. Configuration Management

### 10.1 appsettings Structure
```json
{
  "Logging": {
    "LogLevel": { "Default": "Information" }
  },
  "ConnectionStrings": {
    "SocialMotive": "Server=...",
    "TrekkerGenerator": "Server=..."
  },
  "Authentication": {
    "Authority": "https://oidc-provider.com",
    "ClientId": "client-id",
    "ClientSecret": "secret" // Use user secrets in dev
  },
  "Telerik": {
    "LicenseKey": "key" // Or env var
  },
  "Storage": {
    "Type": "AzureBlob" | "FileSystem",
    "Path": "/uploads" | "blob-connection-string"
  },
  "Features": {
    "AiImageGeneration": false, // Feature flag
    "AuditLogging": true
  }
}
```

### 10.2 Environment Variables (Production)
- Use **Azure Key Vault** or **GitHub Secrets**
- Override appsettings via env vars: `ConnectionStrings__TrekkerGenerator=...`
- Never commit secrets to repo

---

## 11. Performance & Optimization (v1 Baseline, v2+ Enhancements)

### 11.1 Database Query Optimization
- **Lazy loading disabled**, use `.Include()` for related data
- **AsNoTracking()** for read-only queries (admin grids)
- **Batch operations** for bulk deletes (v2+)
- **Query timeout**: 30 sec (adjust per query if needed)

### 11.2 Frontend Performance
- **Lazy loading components** (Suspense in v2+)
- **Debounce search** (500ms, not per keystroke)
- **Canvas rendering**: Canvas 2D API is fast; limit layer count to ~50 (v1), optimize in v2+
- **Bundle size**: Tree-shake unused Telerik modules

### 11.3 Caching (v2+)
- **Response caching**: `[ResponseCache(Duration = 300, VaryByQueryKeys = ["page"])]`
- **Distributed cache**: Redis for session/token validation
- **HTTP cache headers**: ETag, Last-Modified for static assets

---

## 12. Version Control & Branching

### 12.1 Git Strategy
- **Main branch**: Production-ready code (protected, PR review required)
- **Develop branch**: Integration branch for v2
- **Feature branches**: `feature/canvas-editor`, `feature/grid-export`, per task
- **Release branches**: `release/v1.0`, `release/v2.0`, for hotfixes

### 12.2 Commit Convention
```
feat(TrekkerGenerator): add canvas layer reordering
fix(Admin): correct table whitelist bug
docs(API): update endpoint documentation
test(Frontend): add dashboard filter tests
```

---

## 13. Development Workflow

### 13.1 Local Setup
1. Clone repo
2. `dotnet restore`
3. Create `appsettings.Development.json` (override OIDC, DB connection for local SQL)
4. `dotnet ef database update` (apply migrations)
5. `dotnet run --project src/SocialMotive.Frontend.Web`
6. Open https://localhost:5001

### 13.2 Database Migrations
```bash
# Add migration
dotnet ef migrations add AddLayersTable --project src/SocialMotive.TrekkerGenerator.Core

# Apply migrations (auto on startup via Program.cs, or manual)
dotnet ef database update --project src/SocialMotive.TrekkerGenerator.Core
```

### 13.3 Running Tests
```bash
dotnet test tests/
```

---

## 14. Roadmap: v1 → v2+ Technical Debt & Optimization

### v1 (Current)
- Basic CRUD, no advanced patterns (CQRS, event sourcing)
- InMemory caching only
- Synchronous file uploads
- Manual admin table whitelisting

### v2 Enhancements
- CQRS + event sourcing (optional, if needed for audit/history)
- Redis distributed caching
- Async/background file processing (upload to blob)
- Dynamic table whitelist (reflection-based admin exposure)
- Full-text search (Elasticsearch, if needed)
- GraphQL API (alongside REST)
- OpenAPI/Swagger auto-generation

### v3 & Beyond
- Multitenancy support (separate DBs per customer)
- Microservices decomposition (separate render service, AI service)
- Advanced monitoring (custom dashboards, alerts)
- Machine learning (volunteer matching, event recommendations)
