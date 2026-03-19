# Core Model Admin — AI Assistant Instructions

## Purpose
DTOs and request/response models for the Admin feature area.
These models map to Admin API payloads and should stay aligned with `SocialMotive.Core.Data` entities and AutoMapper profiles.

## Location
- Folder: `src/SocialMotive.Core/Model/Admin/`

## Conventions
- One class per file.
- Keep nullable annotations accurate (`?` only when allowed by API contract).
- Add data annotations only when they are required by API validation.
- Keep property names aligned with API payload names and mapping profiles.
- Do not add business logic to model classes.

## System-managed fields
- All `AssignedBy`, `ModifiedBy`, `OwnedBy` properties are readonly.
- All `ModifiedAt`, `CreatedAt`, `AssignedAt` properties are readonly.
- These values are set by server/controller logic, not by clients.

## Mapping alignment
- When adding/changing fields here, update:
  - `src/SocialMotive.Core/Mapping/AdminMappingProfile.cs`
  - Relevant Admin controller partial under `src/SocialMotive.Core/Controllers/Admin/`
  - Any UI usage in `src/SocialMotive.WebApp/Components/Pages/Admin/`

## Notes
- Prefer simple DTOs for transport.
