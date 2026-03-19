# Admin Controllers — AI Assistant Instructions

## Purpose
CRUD API controllers for all admin-managed database tables. All endpoints require the `Admin` role and are routed under `api/admin/`.

## Files in This Directory

| File | Entities Covered |
|------|-----------------|
| `AdminController.cs` | Base partial class — DI constructor only |
| `AdminController.Users.partial.cs` | `DbUser` → `User` DTO |
| `AdminController.Trackers.partial.cs` | `DbTracker` → `Tracker` / `TrackerUpdateRequest` DTOs |
| `AdminController.Events.partial.cs` | `DbEvent` → `Event` DTO |
| `AdminController.EventTypes.partial.cs` | `DbEventType` → `EventType` DTO |
| `AdminController.EventSkills.partial.cs` | `DbEventSkill` → `EventSkill` DTO |
| `AdminController.Labels.partial.cs` | `DbLabel` → `Label` DTO |
| `AdminController.UserSocialAccounts.partial.cs` | `DbUserSocialAccount` → `UserSocialAccount` DTO |
| `AdminController.SocialPlatforms.partial.cs` | `DbSocialPlatform` → `SocialPlatform` DTO |
| `ImportController.cs` | Bulk JSON import for any whitelisted table |

## Conventions

### Structure
- `AdminController` is declared as `partial` — each entity group lives in its own `AdminController.{Entity}.partial.cs` file
- Base class is in `AdminController.cs` — contains constructor and injected dependencies only (`ILogger`, `SocialMotiveDbContext`, `IMapper`)
- New entities always get their own partial file — never add regions to existing partials

### Routes
- BaseRoute: `api/admin`
- Plural: Entity name plural
- Singular: Entity name singular
- Standard CRUD pattern per entity:
  - `GET    {BaseRoute}/{Plural}`          — list all
  - `GET    {BaseRoute}/{Singular}/{id:int}` — get by ID
  - `POST   {BaseRoute}/{Singular}`          — create
  - `PUT    {BaseRoute}/{Singular}/{id:int}` — update
  - `DELETE {BaseRoute}/{Singular}/{id:int}` — delete
- Parent: Singular
- SubResource: Plural
- Sub-resource routes use nested path: `GET {BaseRoute}/{parent}/{id}/{SubResource}` (e.g. `user/{userId}/social-accounts`)

### CRUD Pattern
```csharp
// List
var items = await _dbContext.Entities
    .ProjectTo<EntityDto>(_mapper.ConfigurationProvider)
    .ToListAsync();

// Get by ID
var entity = await _dbContext.Entities.FindAsync(id);
if (entity == null) return NotFound();
return Ok(_mapper.Map<EntityDto>(entity));

// Create
var entity = _mapper.Map<DbEntity>(dto);
entity.Created = DateTime.UtcNow;   // set system-managed fields manually after mapping
entity.Modified = DateTime.UtcNow;
_dbContext.Entities.Add(entity);
await _dbContext.SaveChangesAsync();
return CreatedAtAction(nameof(GetEntity), new { id = entity.EntityId }, _mapper.Map<EntityDto>(entity));

// Update
var entity = await _dbContext.Entities.FindAsync(id);
if (entity == null) return NotFound();
_mapper.Map(dto, entity);
entity.Modified = DateTime.UtcNow;  // set system-managed fields manually after mapping
_dbContext.Entities.Update(entity);
await _dbContext.SaveChangesAsync();
return Ok(_mapper.Map<EntityDto>(entity));

// Delete
var entity = await _dbContext.Entities.FindAsync(id);
if (entity == null) return NotFound();
_dbContext.Entities.Remove(entity);
await _dbContext.SaveChangesAsync();
return Ok();
```

### Error Handling
- Every action wraps in `try/catch`
- Catch returns `BadRequest(new { message = "...", error = ex.Message })`
- Log with `_logger.LogError(ex, "...", id)` — include entity ID in log message where applicable

### Mapping
- Use `ProjectTo<T>()` for list queries (avoids loading full entity graph)
- Use `_mapper.Map<T>(entity)` for single-object reads
- Use `_mapper.Map(dto, entity)` for updates (maps onto existing tracked entity)
- System-managed fields (`Created`, `Modified`, `CreatedAt`, `UpdatedAt`, `PasswordHash`, etc.) are **ignored in mapping profiles** and set manually after mapping

### XML Docs
- Every action must have a `/// <summary>` comment — used by Swagger

## Adding a New Entity

1. Create `AdminController.{Entity}.partial.cs` in this folder
2. Declare `public partial class AdminController` (no base class, no attributes)
3. Add the 5 standard CRUD actions inside `#region {Entity} ... #endregion`
4. Add the corresponding DTO to `SocialMotive.Core/Model/Admin/`
5. Add `CreateMap<DbEntity, EntityDto>()` and reverse map to `AdminMappingProfile`
6. Add service methods to `AdminApiService` in `SocialMotive.Core/Services/`
