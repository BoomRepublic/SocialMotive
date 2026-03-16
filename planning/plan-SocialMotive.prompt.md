# SocialMotive Platform Plan (as-is)

## Context
- Date: 2026-03-11
- Stack: Blazor + Telerik + SQL Server
- Auth: Claims-based role-based access control
- Scope: Single unified web app with role-based feature areas

## App Landscape
- **SocialMotive.Web** (unified app)
  - Production URL: https://socialmotive.net
  - Roles: Volunteer, Organizer, Admin
  - Features by role:
    - Volunteer Dashboard (`/volunteer/events`, `/volunteer/my-events`, `/volunteer/profile`) — for volunteers
    - Organizer Dashboard (`/organizer/events`, `/organizer/my-events`, `/organizer/profile`) — for organizers
    - AssetGenerator Editor (`/generator`) — for volunteers
    - Admin Dashboard (`/admin`) — for admins only
  - API: All endpoints on same `/api` base (https://socialmotive.net/api)
  - All api controllers in same app seperated by roles/areas (no separate apps)
  - AuthZ: Claims-based routing & component visibility (Volunteer, Organizer, Admin roles)

## Libraries
- **SocialMotive.Core**
  - Shared contracts, services, auth, observability
  - Feature-specific namespaces: Web, Generator, Admin
  - Generator domain logic (under SocialMotive.Core.Generator namespace)
- **SocialMotive.Web.Tests** (unit + integration tests)

## Dependencies (project direction)
- **SocialMotive.Web** → SocialMotive.Core (single unified app, all features)
- No app-to-app references (single app architecture)

## API Architecture
- **Single app** hosts all APIs under `/api` base path
- Feature-organized controllers: Volunteers, Organizers, Generator, Admin
- Shared API service layer for all features
- Flow: Blazor component → API service → Controller → Core logic (feature-agnostic)

### API Service Layer (by feature area)
- **Frontend**: `DashboardApiService`, `LookupApiService` → GET /api/dashboard/*, GET /api/lookups
- **Generator**: `TemplatesApiService`, `AssetsApiService`, `RenderApiService` → POST /api/generator/*
- **Admin**: `MetadataApiService`, `TableCrudApiService`, `ExportApiService` → GET/POST /api/admin/*

### Controllers Organization
- All controllers in `SocialMotive.Web/Controllers/`:
  - `VolunteerController.cs` (Volunteer features)
  - `OrganizerController.cs` (Organizer features)
  - `GeneratorController.cs` (Generator features)
  - `AdminController.cs` (Admin features)

### Controller and API service classes (Core-first contract model)
- **SocialMotive.Core** contains shared contracts and base abstractions (in feature namespaces)
- **SocialMotive.Web** contains concrete controllers & API service implementations

#### In SocialMotive.Core
- Api contracts/base:
  - IApiService
  - ApiServiceBase
  - ApiRequestContext
  - ApiErrorResult
- Frontend service contracts:
  - IDashboardApiService
  - ILookupApiService
- AssetGenerator service contracts:
  - ITemplatesApiService
  - IAssetsApiService
  - IRenderApiService
  - IAiApiService
- Admin service contracts:
  - IMetadataApiService
  - ITableCrudApiService
  - IExportApiService

#### In SocialMotive.Web
- **Generator Controllers:**
  - TemplatesController
  - AssetsController
  - RenderController
  - AiController

#### In SocialMotive.Web/Services/Api/
- **Generator service implementations:**
  - TemplatesApiService
  - AssetsApiService
  - RenderApiService
  - AiApiService

### Namespaces

#### SocialMotive.Core
- Contracts/base:
  - SocialMotive.Core.Api
  - SocialMotive.Core.Api.Abstractions
  - SocialMotive.Core.Api.Models
- Volunteer contracts:
  - SocialMotive.Core.Volunteer.Abstractions
  - SocialMotive.Core.Volunteer.Contracts
- Organizer contracts:
  - SocialMotive.Core.Organizer.Abstractions
  - SocialMotive.Core.Organizer.Contracts
- Generator contracts:
  - SocialMotive.Core.Generator.Abstractions
  - SocialMotive.Core.Generator.Contracts
- Admin contracts:
  - SocialMotive.Core.Admin.Abstractions
  - SocialMotive.Core.Admin.Contracts

#### SocialMotive.Web (Unified App)
- Controllers (in SocialMotive.Web/Controllers/):
  - SocialMotive.Web.Controllers.Web (Volunteer/Organizer features)
  - SocialMotive.Web.Controllers.Generator
  - SocialMotive.Web.Controllers.Admin

- API services (in SocialMotive.Web/Services/Api/):
  - SocialMotive.Web.Services.Api.Web
  - SocialMotive.Web.Services.Api.Generator
  - SocialMotive.Web.Services.Api.Admin

## Dto (agreed scope)
- Core canvas: CanvasDocumentDto, CanvasSizeDto, CanvasMetadataDto
- Layers: LayerDto, TextLayerDto, ImageLayerDto
- Assets: UploadAssetRequestDto, UploadAssetResponseDto, AssetDto
- Templates: CreateTemplateRequestDto, UpdateTemplateRequestDto, TemplateSummaryDto, TemplateDetailDto
- Render/export: RenderPngRequestDto, RenderPngResponseDto, ExportOptionsDto
- AI-ready (provider-agnostic): GenerateImageRequestDto, GenerateImageResponseDto
- API standardization: ApiErrorDto, ValidationErrorDto, paging Dto

### DTO Catalog with namespaces

#### Common API Dto
- Namespace: SocialMotive.Core.Api.Contracts
  - ApiEnvelopeDto<T>
  - ApiErrorDto
  - ValidationErrorDto
  - PagedRequestDto
  - PagedResponseDto<T>
  - SortDescriptorDto
  - FilterDescriptorDto

#### Authentication Dto
- Namespace: SocialMotive.Core.Api.Contracts.Identity
  - UserContextDto
  - ClaimDto
  - RoleDto

#### Generator Canvas Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Canvas
  - CanvasDocumentDto
  - CanvasSizeDto
  - CanvasMetadataDto
  - LayerOrderDto

#### Generator Layer Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Layers
  - LayerDto
  - TextLayerDto
  - ImageLayerDto

#### Generator Asset Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Assets
  - UploadAssetRequestDto
  - UploadAssetResponseDto
  - AssetDto
  - AssetListResponseDto

#### Generator Template Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Templates
  - CreateTemplateRequestDto
  - UpdateTemplateRequestDto
  - TemplateSummaryDto
  - TemplateDetailDto
  - TemplateListResponseDto

#### Generator Render Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Rendering
  - RenderPngRequestDto
  - RenderPngResponseDto
  - ExportOptionsDto
  - RenderJobStatusDto

#### Generator AI Dto
- Namespace: SocialMotive.Core.Generator.Contracts.Ai
  - GenerateImageRequestDto
  - GenerateImageResponseDto
  - AiGenerationOptionsDto
  - AiProviderCapabilityDto

#### Admin Dto
- Namespace: SocialMotive.Core.Admin.Contracts
  - TableDescriptorDto
  - ColumnDescriptorDto
  - TableListResponseDto
  - GridQueryDto
  - RowDto
  - UpsertRowRequestDto
  - DeleteRowRequestDto
  - ExportRequestDto
  - ExportResultDto

## Entity Model (generator)
- Users
- Templates
- Assets
- Layers
- RenderJobs

## Entity Models (socialmotive database - existing structure used by apps)

### Existing tables (volunteers/events domain)
- Users (existing, primary user record)
- Trackers (volunteer/tracker mapping)
- Events, EventTypes, EventTasks, EventTaskAssignments
- EventParticipants (volunteer participation in events)
- Groups, Labels, TrackerLabels, TrackerRoles
- Locations (GPS tracking)
- Cities, Invites, Settings, UserAccounts

### SocialMotive.Web entity mapping (Frontend feature)
- Frontend (volunteer portal) reads Users, Events, EventParticipants, Trackers, Groups, Labels - **NO new tables required**.
- Uses existing Users + role/group assignment for volunteers.
- May add new table: `AuditLogs` (optional, for system-wide audit trail).

### SocialMotive.Web Admin entity mapping
- Admin feature consumes **all tables** from both SocialMotive + AssetGenerator databases via whitelist CRUD grids.
- May add or use existing: `AuditLogs` to track admin mutations (who changed what, when).
- No separate domain tables; Admin is a "super-user data management interface".

### New optional table (both apps, if audit needed)
- AuditLogs (UserId FK, TableName, Operation, OldValues, NewValues, Timestamp, IPAddress)

### User model (local database only)
- **Existing `Users` table** in SocialMotive stores all user data:
  - FirstName, LastName, Email, PasswordHash, ProfileImage, Bio, etc.
  - Roles assigned via UserRoles table (Volunteer, Organizer, Admin)
  - No external subject ID mapping needed; users authenticate via local account

### Dto for Frontend (volunteers)
- Namespace: SocialMotive.Core.Web.Contracts
  - EventDto (summary + details)
  - EventParticipantDto (registration status/hours/review)
  - UserProfileDto (own profile + settings)
  - TrackerDto (volunteer info)
  - GroupDto, LabelDto (categorization)
  - EventListResponseDto (paged events)

### Dto for Admin (CRUD grids)
- Namespace: SocialMotive.Core.Admin.Contracts
  - (Reuse existing entity Dto from Core libraries)
  - GridDataRequestDto (filtering, sorting, paging)
  - GridDataResponseDto<T> (paged result)
  - TableMetadataDto (column info, type, constraints)
  - AuditLogDto (optional, for audit trail display)

## Platform Context
- SocialMotive: online social platform for volunteers + event organizers
- MVP: event matching + registration + AssetGenerator promo tool
- Database: existing volunteer tracking + events system being extended

## Databases
- Generator DB: assetgenerator
- Admin + Frontend DB: socialmotive

### Naming convention
- Follow SocialMotive DB naming style conventions for new scripts/objects.

## Existing SocialMotive DB Script
- Existing script provided for SocialMotive is the style baseline.
- Keep legacy-compatible SQL style for AssetGenerator DB script as requested.

## SQL Script Requirement
- Create file required: Sql/create.sql
- Target: MS SQL Server
- AssetGenerator tables in v1:
  - Users
  - Templates
  - Assets
  - Layers
  - RenderJobs

## Authentication and Security
- Protocol: Claims-based role-based access control
- User model: local Users table only
- Authorization source: user database roles
- Tenant model: single tenant
- No external SSO configuration required

## Admin Requirements
- In SocialMotive.Web: pages for all DB tables (both socialmotive + assetgenerator)
- UI: Telerik editable grids
- Required actions: Read, Create, Update, Delete, Export (CSV/Excel)

## Telerik MCP Requirement
- Telerik MCP must be used for admin grid implementation guidance/configuration.
- Apply MCP-driven patterns for:
  - Grid setup
  - Edit mode
  - Validation/editor bindings
  - Paging/sorting/filtering
  - Export setup

## Open Implementation Notes
- Keep whitelist-based table exposure for admin CRUD APIs.
- Enforce claims-based admin policies.
- Add audit logging for admin data mutations.
- Keep no frontmatter in this file (intentional).

## v2+ Feature Backlog (Nice-to-Haves for Future Releases)

### Social Network Features
- User feed/timeline (posts, event updates, connections)
- Direct messaging system (1-on-1 chat, group chats)
- Notification system (event updates, messages, mentions, follows)
- User connections/followers (volunteer follows organizer, vice versa)
- User discovery (search, filter by skills/interests/location)
- User ratings/reviews (post-event bidirectional)
- Badges/achievements (volunteer badges for hours, event count, skills)
- User profiles as "organizations" (team, NGO, company profiles)

### Content & Community
- User posts/blog articles
- Event discussion threads
- Photo/media gallery per event
- User testimonials/case studies
- Community guidelines enforcement
- Content moderation dashboard

### Event Management (Advanced)
- Event templates
- Recurring/series events
- Waitlist management
- Skills-based matching (volunteer skills vs event requirements)
- Event surveys/feedback
- Capacity forecasting
- Event budgeting + cost tracking

### AssetGenerator v2+
- AI image generation (integrate OpenAI/Stability/etc)
- Advanced layer animations
- Brand kit management (logos, colors, fonts per organizer)
- Template sharing marketplace
- Batch PNG export (multiple variations)
- Video/animation export
- Design collaboration (realtime editing)
- Mobile app version

### Analytics & Reporting
- Volunteer hours analytics
- Event ROI/impact measurement
- User engagement analytics
- Organizer dashboard (event performance, volunteer retention)
- System-wide reports (admin)

### Gamification
- Leaderboards (hours, events attended, ratings)
- Point system (volunteer earnings via point redemption)
- Challenges (e.g., "attend 5 events in Q1")

### Marketplace/Monetization
- Skill marketplace (freelance gigs within platform)
- Monetized template store (AssetGenerator designs)
- In-app payments (event fees, donations)
- Organizer premium tiers

### Mobile
- Native iOS/Android apps
- Offline event browsing
- Push notifications
- Mobile-optimized AssetGenerator

### Integrations
- Calendar sync (Google Cal, Outlook)
- Social media sharing (auto-post events to Twitter/LinkedIn)
- Payment processor integrations (Stripe, PayPal)
- Email marketing integration
- CRM integration (Salesforce)
- SMS notifications

### Admin/Moderation
- Automated content flagging (spam, offensive)
- User ban/suspension workflows
- Event approval workflows (automated + manual)
- Dispute resolution system
- GDPR/data export tools

### Accessibility & Internationalization
- Multi-language support (i18n)
- Accessibility audit (WCAG 2.1 AA compliance)
- Dark mode

### Performance & Infrastructure
- CDN for static assets
- Caching layer (Redis)
- Full-text search (Elasticsearch)
- Background job queue (background processing)
- Load testing + optimization

### Security Enhancements
- Two-factor authentication (2FA)
- OAuth/OpenID provider (SocialMotive as auth provider for others)
- API rate limiting
- DDoS protection
- Penetration testing

### Documentation & DevOps
- API documentation (OpenAPI/Swagger maintained)
- User onboarding guides (video tutorials)
- Organizer getting-started playbook
- CI/CD automation (GitHub Actions, Azure Pipelines)
- Infrastructure as Code (Terraform/Bicep)
- Monitoring/observability (Application Insights, custom dashboards)