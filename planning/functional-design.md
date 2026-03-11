# SocialMotive Platform - Functional Design

## 1. SocialMotive.Frontend.Web (Volunteer Portal)

### 1.1 Core Users
- **Volunteer**: searches, registers, attends events, tracks hours
- **Organizer**: creates events, manages volunteers, tracks impact
- **Both roles**: own profile, connections, ratings/reviews (v2+)

### 1.2 Main Features (MVP)

#### 1.2.1 Authentication & User Management
- **Flow**: SSO redirect → OIDC provider → callback → create/update local User
- **User Profile Screen**:
  - View/edit: firstName, lastName, email, profileImage, bio, phone
  - Display: joined date, hours logged, events attended
  - Role indicator (Volunteer / Organizer / Admin)
- **Sign In**: SSO login button (OIDC redirect)
- **Sign Out**: clear local auth, OIDC signout

#### 1.2.2 Event Discovery (Volunteer Dashboard)
- **Event List Screen**:
  - Query: `GET /api/dashboard/events` (filtered, paged)
  - Display: Event cards (title, date, organizer, location, skill tags, volunteers needed)
  - Filters: Date range, location, skill required, search text
  - Sorting: nearest date, most popular, newest
  - Pagination: 10 items/page
- **Event Detail Screen**:
  - Full event info: description, schedule, tasks, location, map
  - Organizer card: name, photo, rating, past events
  - Task list: required tasks + sign-up button per task
  - Volunteer list (anonymized): count + avatars
  - Action buttons: "Register", "Share", "Save" (v2+)

#### 1.2.3 Event Registration
- **Registration Flow**:
  - `POST /api/dashboard/events/{eventId}/register`
  - Volunteer selects task(s) → confirm → create EventParticipant record
  - Success: "You're registered! Check email for details"
- **My Events Screen**:
  - Upcoming: events user is registered for (with status)
  - Past: completed events (with hours, rating option)
  - Action: view details, "Mark Complete", rate organizer

#### 1.2.4 Hours Tracking
- **For Volunteers**:
  - View: total hours logged (summary on profile)
  - Organizer marks hours after event → volunteer sees update
- **For Organizers**:
  - View event participants + manual hour entry
  - `POST /api/dashboard/events/{eventId}/participants/{participantId}/log-hours`

#### 1.2.5 Notifications (v1 basic, full v2+)
- **v1 MVP**: No real-time, but email/redirect on registration success
- **v2+ planned**: Push notifications, in-app bell icon, notification center

### 1.3 API Endpoints (Frontend context)

```
Authentication (via OIDC, no explicit API endpoints)
GET  /signin-oidc → OIDC callback
GET  /signout-oidc → Logout

Dashboard
GET  /api/dashboard/user-profile         → UserProfileDto
GET  /api/dashboard/events               → PagedResponseDto<EventDto>
GET  /api/dashboard/events/{eventId}     → EventDetailDto
GET  /api/dashboard/my-events            → PagedResponseDto<EventDto>

Registration
POST /api/dashboard/events/{eventId}/register
  Body: { taskIds: int[] }
  → EventParticipantDto

Hours
POST /api/dashboard/events/{eventId}/participants/{participantId}/log-hours
  Body: { hoursWorked: decimal, notes: string }
  → EventParticipantDto

Ratings (v2+)
POST /api/dashboard/events/{eventId}/rate
  Body: { rating: int, review: string }
```

### 1.4 Key Business Rules
- Volunteer can register for only 1 task per event
- Max participants per task cannot be exceeded
- Organizer must manually approve (or auto-approve) before participant is final
- Hours logged only by organizer or trusted admin

---

## 2. SocialMotive.TrekkerGenerator.Web (Promo Canvas Editor)

### 2.1 Core User
- **Organizer**: creates promotional images for events (posters, social media)

### 2.2 Main Features (MVP)

#### 2.2.1 Canvas Workspace
- **Canvas Editor Page**:
  - Left sidebar: layer panel (list of text/image layers, add/remove/reorder)
  - Center: WYSIWYG canvas (drag, resize, rotate layers)
  - Right sidebar: style controls (font, color, opacity, alignment)

#### 2.2.2 Layer Management
- **Add Layer**:
  - "Add Text" → text input, default font/size/color
  - "Add Image" → file upload or URL, dimensions
  - Each layer: name, type, x/y/width/height, rotation, opacity, z-index, visibility toggle
- **Manage Layer**:
  - Drag to reorder z-order
  - Click to select & show style controls
  - Delete, duplicate, lock/unlock (v2+)

#### 2.2.3 Text Styling
- **Controls**:
  - Font family (dropdown: Arial, Georgia, custom Google Fonts in v2+)
  - Font size (px, input field)
  - Font weight (bold/normal/light selector)
  - Color (color picker, hex input)
  - Alignment (left/center/right buttons)
  - Line height (spacing slider)
  - Shadow/outline (optional, advanced v2+)

#### 2.2.4 Background/Assets
- **Upload Background Image**:
  - `POST /api/trekker/assets/upload`
  - Accept: PNG, JPG, WebP
  - Store: server-side path or blob URL
  - Display: on canvas as bottom layer
- **Transparent PNG Support**:
  - Preserve alpha channel in export
  - Show checkerboard behind transparent areas in canvas

#### 2.2.5 Canvas Size & Export
- **Canvas Preset**:
  - Dropdown: Social Media sizes (Instagram 1080×1080, LinkedIn 1200×627, etc.)
  - Custom: width/height input
- **Export PNG**:
  - `POST /api/trekker/render`
  - Input: canvas document state (layers, positions, styles)
  - Output: PNG blob (transparent or white background option)
  - Download as: `EventName_Poster_[timestamp].png`

#### 2.2.6 Template Save/Load
- **Save Template**:
  - `POST /api/trekker/templates`
  - Input: name, canvas document (JSON)
  - Create Template record
- **Load Template**:
  - Dropdown: "My Templates" list
  - Select → populate canvas from template
- **Template Management**:
  - List screen: name, thumbnail, date created, actions (load, rename, delete)
  - `GET /api/trekker/templates`
  - `PUT /api/trekker/templates/{id}`
  - `DELETE /api/trekker/templates/{id}`

### 2.3 API Endpoints (TrekkerGenerator context)

```
Assets
POST /api/assets/upload                       → AssetDto
GET  /api/assets/{assetId}
DELETE /api/assets/{assetId}

Templates
GET  /api/templates                           → PagedResponseDto<TemplateSummaryDto>
GET  /api/templates/{id}                      → TemplateDetailDto
POST /api/templates                           → TemplateSummaryDto
PUT  /api/templates/{id}                      → TemplateDetailDto
DELETE /api/templates/{id}

Render/Export
POST /api/render                              → PNG blob (or RenderResultDto)
  Body: { canvasDocument: CanvasDocumentDto, exportOptions: ExportOptionsDto }

AI Generation (v2+, placeholder)
POST /api/ai/generate-image
  Body: { prompt: string, style: string }
  → AiGenerationResponseDto
```

### 2.4 Canvas Document Structure (Internal JSON)
```json
{
  "canvasId": "uuid",
  "name": "Event Poster 2026",
  "width": 1080,
  "height": 1080,
  "backgroundColor": "#ffffff",
  "layers": [
    {
      "layerId": "uuid",
      "type": "image",
      "name": "Background",
      "assetId": "uuid",
      "x": 0,
      "y": 0,
      "width": 1080,
      "height": 1080,
      "rotation": 0,
      "opacity": 1.0,
      "zIndex": 0,
      "isVisible": true
    },
    {
      "layerId": "uuid",
      "type": "text",
      "name": "Event Title",
      "text": "Volunteer Day 2026",
      "fontFamily": "Arial",
      "fontSize": 48,
      "fontWeight": "bold",
      "color": "#ffffff",
      "alignment": "center",
      "x": 100,
      "y": 400,
      "width": 880,
      "height": 100,
      "rotation": 0,
      "opacity": 1.0,
      "zIndex": 1,
      "isVisible": true
    }
  ],
  "createdUtc": "2026-03-11T10:00:00Z",
  "updatedUtc": "2026-03-11T10:15:00Z"
}
```

### 2.5 Key Business Rules
- Canvas must have 1+ layers
- Text max length: 500 chars
- Image max size: 50 MB
- Export max resolution: 4000×4000 px
- Template name max: 100 chars
- User can have max 50 templates (v1, increase in v2)

---

## 3. SocialMotive.AdminBackend.Web (Data Management)

### 3.1 Core User
- **Admin**: browse, search, create, update, delete records across both databases

### 3.2 Main Features (MVP)

#### 3.2.1 Table Listing & Discovery
- **Table Browser Screen**:
  - Left sidebar: database selector (SocialMotive / TrekkerGenerator)
  - Main: table list (all user tables, exclude system tables)
  - Per table: row count, last modified, actions (view, export)
    - Whitelisted tables only (e.g., NOT `sys.*`, NOT sensitive internal tables)

#### 3.2.2 Data Grid & CRUD
- **Grid Screen** (per table):
  - Telerik editable grid with columns from metadata
  - Features:
    - **Paging**: 25 rows/page, configurable
    - **Sorting**: multi-column sort
    - **Filtering**: column-based, operator dropdowns (equals, contains, greater than, etc.)
    - **Inline Edit**: double-click cell → inline editor (text, date picker, dropdown based on column type)
    - **Row Actions**: 
      - Edit details (full form popup)
      - Delete (confirm dialog)
      - Duplicate (v2+)
  - **Create New Row**:
    - Button "Add Row" → blank row at bottom OR popup form
    - Form: all columns, required field validation
    - Submit → `POST /api/admin/crud/[TableName]`
  - **Bulk Actions** (v2+):
    - Select checkboxes → Delete Selected, Export Selected

#### 3.2.3 Table Metadata & Validation
- **Metadata API** (internal):
  - `GET /api/admin/metadata/tables` → list of TableDescriptorDto
  - Per table: column list (name, type, nullable, pk, fk, max length)
  - Per column: editor type (text, number, date, dropdown, toggle)
  - Dropdowns auto-populated from FK relations (e.g., CategoryId dropdown loads categories)

#### 3.2.4 Export
- **Export Grid Data**:
  - Button: "Export" → format selector (CSV, Excel)
  - Action: `POST /api/admin/export`
  - Output: CSV/XLSX file download with current filters applied
  - Columns: all visible columns in grid

#### 3.2.5 Audit Logging
- **Audit Log Screen** (optional v1, likely v2+):
  - Table: Admin → Admin Activity
  - Columns: user, table, operation (INSERT/UPDATE/DELETE), timestamp, old values, new values
  - Search: by user, date range, table, operation type

### 3.3 API Endpoints (Admin context)

```
Metadata
GET  /api/admin/metadata/tables               → TableListResponseDto
GET  /api/admin/metadata/tables/{name}        → TableDescriptorDto
GET  /api/admin/metadata/columns/{table}      → ColumnDescriptorDto[]

CRUD
GET  /api/admin/crud/{tableName}              → GridDataResponseDto<RowDto>
  Query: ?page=1&pageSize=25&sort=Name desc&filter=Status eq 'Active'
POST /api/admin/crud/{tableName}              → RowDto
  Body: { field1: value1, field2: value2 }
PUT  /api/admin/crud/{tableName}/{id}         → RowDto
  Body: { field1: newValue1 }
DELETE /api/admin/crud/{tableName}/{id}

Export
POST /api/admin/export                        → file blob (CSV/XLSX)
  Body: { tableName: string, format: 'csv'|'xlsx', filters: ... }

Audit (v2+ likely)
GET  /api/admin/audit-log                     → PagedResponseDto<AuditLogDto>
  Query: ?tableName=Users&userId=uuid&startDate=...
```

### 3.4 Whitelisted Tables (Example)

**SocialMotive DB:**
- Users, Trackers, TrackerRoles, Groups, Labels, Cities
- Events, EventTypes, EventTasks, EventParticipants
- AuditLogs (admin-only)
- Settings (admin-only)

**TrekkerGenerator DB:**
- Users (SSO users), Templates, Assets
- NOT: Layers, RenderJobs (internal/transient, v2+ could expose)

**NOT whitelisted (hidden from admin):**
- Any `sys.*` tables
- Any internal/shadow tables (v2+ will refine)

### 3.5 Key Business Rules
- Admin can CRUD only whitelisted tables
- DELETE cascades apply per DB schema (e.g., deleting User cascades to templates/assets)
- Audit log auto-created per mutation (user, table, operation, timestamp, old/new values)
- File exports limited to 100k rows (v1), streaming in v2+
- Admin role checked via SSO claims (upstream defines who is admin)

---

## 4. Cross-App Workflows

### 4.1 Event Promotion Flow
1. **Organizer** creates event in Frontend (existing system flow)
2. **Organizer** opens TrekkerGenerator to design poster
3. **Organizer** uploads event image or uses template
4. **Organizer** styles text (event name, date, "Register Now!")
5. **Organizer** exports PNG → downloads poster
6. **Organizer** shares poster on social media (manual, v2+ auto-share)
7. **Volunteer** sees event in Frontend, registers

### 4.2 Data Governance Flow
1. **Admin** logs in → AdminBackend
2. **Admin** browses Users table → filters by signup date
3. **Admin** exports active users as CSV for outreach campaign
4. **Admin** notices spam user → clicks Delete → audit logged
5. **Admin** views audit log to confirm deletion + who did it

### 4.3 Profile Update Flow
1. **Volunteer** logs in → Frontend
2. **Volunteer** clicks "Edit Profile"
3. **Volunteer** updates bio/skills
4. **System** sanitizes input, validates
5. **System** updates Users table (via API service)
6. **Admin** (optional) sees change in audit log

---

## 5. Data Flow & Consistency

### 5.1 Frontend ↔ API Service ↔ Controller
- All Blazor components call API services (no direct DB access)
- API services call controllers via HttpClient
- Controllers call Core/Application logic (repositories, use-cases)
- Returns DTOs only (no entity objects over HTTP)

### 5.2 SSO User Synchronization
- User lands on SocialMotive.net with SSO token
- Frontend checks `/api/user/profile` (auto-syncs user if missing)
- If ExternalSubjectId not in Users table → create record (upsert)
- SSO claims (name, email, roles) populate/update local User

### 5.3 Admin Changes Audit
- Admin updates Users.Email via grid
- API logs: `INSERT AuditLog (userId, operation='UPDATE', tableName='Users', oldValues='{Email: old@xxx}', newValues='{Email: new@xxx}')`
- Admin opens audit log, sees full history

---

## 6. Error Handling & Validation

### 6.1 Client-Side (Blazor)
- Form validation (required fields, email format, max length)
- Show toast notifications (success, error, warning)
- Disable buttons during API calls (prevent double-submit)

### 6.2 Server-Side (API)
- Validate DTOs (FluentValidation or DataAnnotations)
- Check authorization (claims-based, admin-only endpoints)
- Handle concurrency (optimistic locking if needed)
- Return meaningful error codes (400 Bad Request, 403 Forbidden, 409 Conflict, 500 Internal Error)
- Log errors to app insights + audit trail

### 6.3 Response Format
```json
{
  "success": true,
  "data": { ... },
  "errors": null,
  "correlationId": "uuid"
}
```

OR on error:
```json
{
  "success": false,
  "data": null,
  "errors": {
    "fieldName": ["Field is required"]
  },
  "correlationId": "uuid"
}
```

---

## 7. Performance Expectations

### 7.1 Frontend
- Event list load: < 2 sec (cached if possible)
- Canvas render: real-time < 100ms per layer update
- PNG export: < 5 sec for 1080×1080 image

### 7.2 Admin
- Table load (1k rows): < 3 sec
- Search/filter: < 1 sec
- CSV export (10k rows): < 10 sec

### 7.3 Database
- All queries indexed on FK/search columns
- Paging enforced (no unbounded queries)
- Connection pooling configured

---

## 8. Security Considerations

### 8.1 Authentication
- OIDC/OAuth2 via upstream provider
- JWT tokens for API calls (Bearer scheme)
- Token expiry + refresh handled by OIDC middleware

### 8.2 Authorization
- Frontend: volunteer/organizer roles (read events, own profile edit)
- Admin: admin role only (full CRUD on whitelisted tables)
- Row-level security: users can't edit other users' profiles (v1 MVP, fine-grained v2+)

### 8.3 Data Protection
- SQL parameterized queries (entity framework handles)
- Admin API whitelist prevents access to non-whitelisted tables
- Audit logging on all mutations
- PII: profile images stored server-side, no exposed file paths

### 8.4 API Rate Limiting (v2+)
- Per-user rate limits (e.g., 100 req/min for standard, 1000 for admin)
- Distributed rate limiter (Redis or similar)

---

## 9. Deployment & Configuration

### 9.1 Environment-Specific Config (appsettings)
- OIDC provider URL + client ID
- Database connection strings
- API base URLs (for service-to-controller calls)
- Logging level (Info/Debug)
- Feature flags (Telerik MCP usage, AI provider, etc.)

### 9.2 Release Plan
- **v1 (Beta, this sprint)**:
  - Frontend: event list + register flow
  - TrekkerGenerator: canvas editor + PNG export + template save/load
  - Admin: CRUD grids for core tables
  - SSO integration complete
  - No AI, no messaging, no gamification
  
- **v2 (Post-Beta)**:
  - Social: feed, messaging, followers
  - TrekkerGenerator: AI image gen, animations, collaboration
  - Advanced admin: audit log UI, user bans, event approval workflows
  - Analytics & reporting
  
- **v3+ (Roadmap)**:
  - Mobile apps
  - Integrations (Stripe, email marketing)
  - Marketplace
  - Advanced gamification

---

## Appendix: Key Terminology

- **Tracker**: Volunteer record (links to User)
- **Event**: activity organized by an Organizer
- **EventTask**: sub-component of an Event (e.g., "Setup", "Check-in", "Cleanup")
- **EventParticipant**: volunteer registration for an event
- **Canvas**: the editable design surface in TrekkerGenerator
- **Layer**: text or image element on canvas (z-ordered, styleable)
- **Template**: saved canvas state (layout + styling) for reuse
- **Render Job**: export request for a canvas to PNG
- **Audit Log**: immutable record of admin mutations
