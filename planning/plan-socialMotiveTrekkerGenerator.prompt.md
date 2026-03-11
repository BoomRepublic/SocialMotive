# SocialMotive Platform Plan (as-is)

## Context
- Date: 2026-03-11
- Stack: Blazor + Telerik + SQL Server
- SSO: OIDC/OAuth2 against larger SocialMotive system
- Scope: separate subsystem apps with own APIs

## App Landscape
- SocialMotive.Frontend.Web
  - Production URL: https://socialmotive.net
  - Own API: yes (same host)
- SocialMotive.TrekkerGenerator.Web
  - Production URL: https://trekkergenerator.socialmotive.net
  - API base: https://trekkergenerator.socialmotive.net/api
  - Purpose: image/trekker generator
- SocialMotive.AdminBackend.Web
  - Production URL: https://admin.socialmotive.net
  - Own API: yes (same host)

## Libraries
- SocialMotive.Core
  - Shared cross-app contracts/utilities/auth helpers/observability
- SocialMotive.TrekkerGenerator.Core
  - Merged generator domain + application + infrastructure logic
- SocialMotive.TrekkerGenerator.Web.Tests

## Dependencies (project direction)
- SocialMotive.Frontend.Web -> SocialMotive.Core
- SocialMotive.AdminBackend.Web -> SocialMotive.Core
- SocialMotive.TrekkerGenerator.Web -> SocialMotive.Core, SocialMotive.TrekkerGenerator.Core
- No direct app-to-app project references

## API Architecture
- Each app has its own API controllers.
- Each app has its own API service layer against its own controllers.
- Flow (all apps): Blazor component -> API service -> Controller -> Core logic.

### API Service Layer by app
- SocialMotive.Frontend.Web: UI uses Frontend API services only.
- SocialMotive.TrekkerGenerator.Web: UI uses TrekkerGenerator API services only.
- SocialMotive.AdminBackend.Web: UI uses Admin API services only.

### Controller and API service classes (Core-first contract model)
- SocialMotive.Core contains shared contracts and base abstractions.
- Web projects contain concrete controllers.

#### In SocialMotive.Core
- Api contracts/base:
  - IApiService
  - ApiServiceBase
  - ApiRequestContext
  - ApiErrorResult
- Frontend service contracts:
  - IDashboardApiService
  - ILookupApiService
- TrekkerGenerator service contracts:
  - ITemplatesApiService
  - IAssetsApiService
  - IRenderApiService
  - IAiApiService
- Admin service contracts:
  - IMetadataApiService
  - ITableCrudApiService
  - IExportApiService

#### In SocialMotive.Frontend.Web
- Controllers:
  - DashboardController
  - LookupController
- API service implementations:
  - DashboardApiService
  - LookupApiService

#### In SocialMotive.TrekkerGenerator.Web
- Controllers:
  - TemplatesController
  - AssetsController
  - RenderController
  - AiController
- API service implementations:
  - TemplatesApiService
  - AssetsApiService
  - RenderApiService
  - AiApiService

#### In SocialMotive.AdminBackend.Web
- Controllers:
  - MetadataController
  - TableCrudController
  - ExportController
- API service implementations:
  - MetadataApiService
  - TableCrudApiService
  - ExportApiService

### Namespaces

#### SocialMotive.Core
- Contracts/base:
  - SocialMotive.Core.Api
  - SocialMotive.Core.Api.Abstractions
  - SocialMotive.Core.Api.Models
- Frontend contracts:
  - SocialMotive.Core.Frontend.Abstractions
  - SocialMotive.Core.Frontend.Contracts
- TrekkerGenerator contracts:
  - SocialMotive.Core.TrekkerGenerator.Abstractions
  - SocialMotive.Core.TrekkerGenerator.Contracts
- Admin contracts:
  - SocialMotive.Core.Admin.Abstractions
  - SocialMotive.Core.Admin.Contracts

#### SocialMotive.Frontend.Web
- Controllers:
  - SocialMotive.Frontend.Web.Controllers
- API services:
  - SocialMotive.Frontend.Web.Services.Api

#### SocialMotive.TrekkerGenerator.Web
- Controllers:
  - SocialMotive.TrekkerGenerator.Web.Controllers
- API services:
  - SocialMotive.TrekkerGenerator.Web.Services.Api

#### SocialMotive.AdminBackend.Web
- Controllers:
  - SocialMotive.AdminBackend.Web.Controllers
- API services:
  - SocialMotive.AdminBackend.Web.Services.Api

## DTOs (agreed scope)
- Core canvas: CanvasDocumentDto, CanvasSizeDto, CanvasMetadataDto
- Layers: LayerDto, TextLayerDto, ImageLayerDto
- Assets: UploadAssetRequestDto, UploadAssetResponseDto, AssetDto
- Templates: CreateTemplateRequestDto, UpdateTemplateRequestDto, TemplateSummaryDto, TemplateDetailDto
- Render/export: RenderPngRequestDto, RenderPngResponseDto, ExportOptionsDto
- AI-ready (provider-agnostic): GenerateImageRequestDto, GenerateImageResponseDto
- API standardization: ApiErrorDto, ValidationErrorDto, paging DTOs

### DTO Catalog with namespaces

#### Common API DTOs
- Namespace: SocialMotive.Core.Api.Contracts
  - ApiEnvelopeDto<T>
  - ApiErrorDto
  - ValidationErrorDto
  - PagedRequestDto
  - PagedResponseDto<T>
  - SortDescriptorDto
  - FilterDescriptorDto

#### Identity/SSO DTOs
- Namespace: SocialMotive.Core.Api.Contracts.Identity
  - UserContextDto
  - ClaimDto
  - RoleDto

#### Frontend DTOs
- Namespace: SocialMotive.Core.Frontend.Contracts
  - DashboardSummaryDto
  - LookupItemDto
  - LookupResponseDto

#### TrekkerGenerator Canvas DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Canvas
  - CanvasDocumentDto
  - CanvasSizeDto
  - CanvasMetadataDto
  - LayerOrderDto

#### TrekkerGenerator Layer DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Layers
  - LayerDto
  - TextLayerDto
  - ImageLayerDto

#### TrekkerGenerator Asset DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Assets
  - UploadAssetRequestDto
  - UploadAssetResponseDto
  - AssetDto
  - AssetListResponseDto

#### TrekkerGenerator Template DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Templates
  - CreateTemplateRequestDto
  - UpdateTemplateRequestDto
  - TemplateSummaryDto
  - TemplateDetailDto
  - TemplateListResponseDto

#### TrekkerGenerator Render DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Rendering
  - RenderPngRequestDto
  - RenderPngResponseDto
  - ExportOptionsDto
  - RenderJobStatusDto

#### TrekkerGenerator AI DTOs
- Namespace: SocialMotive.Core.TrekkerGenerator.Contracts.Ai
  - GenerateImageRequestDto
  - GenerateImageResponseDto
  - AiGenerationOptionsDto
  - AiProviderCapabilityDto

#### Admin DTOs
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
- Users (SSO link table with external subject id)
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

### SocialMotive.Frontend.Web entity mapping
- Frontend (volunteer portal) reads Users, Events, EventParticipants, Trackers, Groups, Labels - **NO new tables required**.
- Uses existing SSO Users + role/group assignment for volunteers.
- May add new table: `AuditLogs` (optional, for system-wide audit trail).

### SocialMotive.AdminBackend.Web entity mapping
- Admin backend consumes **all tables** from both SocialMotive + TrekkerGenerator databases via whitelist CRUD grids.
- May add or use existing: `AuditLogs` to track admin mutations (who changed what, when).
- No separate domain tables; Admin is a "super-user data management interface".

### New optional table (both apps, if audit needed)
- AuditLogs (UserId FK, TableName, Operation, OldValues, NewValues, Timestamp, IPAddress)

### User mapping (SSO integration)
- **Existing `Users` table** in SocialMotive will be extended/mapped:
  - Add column: `ExternalSubjectId` (nullable, for OIDC/SSO subject claim)
  - Keep existing: FirstName, LastName, Email, PasswordHash, ProfileImage, Bio, etc.
  - SSO login will upsert Users on first authentication via ExternalSubjectId.

### DTOs for Frontend (volunteers)
- Namespace: SocialMotive.Core.Frontend.Contracts
  - EventDto (summary + details)
  - EventParticipantDto (registration status/hours/review)
  - UserProfileDto (own profile + settings)
  - TrackerDto (volunteer info)
  - GroupDto, LabelDto (categorization)
  - EventListResponseDto (paged events)

### DTOs for Admin (CRUD grids)
- Namespace: SocialMotive.Core.Admin.Contracts
  - (Reuse existing entity DTOs from Core libraries)
  - GridDataRequestDto (filtering, sorting, paging)
  - GridDataResponseDto<T> (paged result)
  - TableMetadataDto (column info, type, constraints)
  - AuditLogDto (optional, for audit trail display)

## Platform Context
- SocialMotive: online social platform for volunteers + event organizers
- MVP: event matching + registration + TrekkerGenerator promo tool
- Database: existing volunteer tracking + events system being extended

## Databases
- Generator DB: trekkergenerator
- Admin + Frontend DB: socialmotive

### Naming convention
- Follow SocialMotive DB naming style conventions for new scripts/objects.

## Existing SocialMotive DB Script
- Existing script provided for SocialMotive is the style baseline.
- Keep legacy-compatible SQL style for TrekkerGenerator DB script as requested.

## SQL Script Requirement
- Create file required: Sql/create.sql
- Target: MS SQL Server
- TrekkerGenerator tables in v1:
  - Users
  - Templates
  - Assets
  - Layers
  - RenderJobs

## SSO and Security
- Protocol: OIDC/OAuth2
- User model: local Users table + external subject id mapping
- Authorization source: upstream SSO claims
- Tenant model: single tenant
- Redirect URIs to configure:
  - https://socialmotive.net/signin-oidc
  - https://trekkergenerator.socialmotive.net/signin-oidc
  - https://admin.socialmotive.net/signin-oidc
- Also configure post-logout redirect URIs for all three.

## Admin Requirements
- In SocialMotive.AdminBackend.Web: pages for all DB tables (both socialmotive + trekkergenerator)
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

### TrekkerGenerator v2+
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
- Monetized template store (TrekkerGenerator designs)
- In-app payments (event fees, donations)
- Organizer premium tiers

### Mobile
- Native iOS/Android apps
- Offline event browsing
- Push notifications
- Mobile-optimized TrekkerGenerator

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