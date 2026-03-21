# DesignService — Implementation Spec

## Purpose
`DesignService` (Core) generates images by composing templates, layers, filters, and assets. It is the business logic layer behind the `SocialMotive.Designer` app and exposes its contract via `IDesignService`.

## Existing Data Model (Generator namespace)

All entities are already in `SocialMotive.Core.Data.Generator` and mapped in `SocialMotiveDbContext`.

| Entity | Table | Key Fields |
|---|---|---|
| `DbTemplate` | `GeneratorTemplate` | `TemplateId`, `UserId`, `Name`, `Width`, `Height`, `IsPublished`, `IsTemplate`, `Tags`, `Category` |
| `DbLayer` | `GeneratorLayer` | `LayerId`, `TemplateId`, `LayerType`, `Name`, `ZIndex`, `AssetId`, `SettingsJson` |
| `DbAsset` | `GeneratorAsset` | `AssetId`, `UserId`, `FileName`, `ImagePng` (bytes), `ImageMetaDataJson`, `Tags`, `IsPublic` |
| `DbRenderJob` | `GeneratorRenderJob` | `RenderJobId`, `TemplateId`, `UserId`, `Status`, `ImagePng` (result), `ErrorMessage`, `StartedAt`, `CompletedAt` |

## Layer Types

`DbLayer.LayerType` string values — controls how the layer is rendered:

| Value | Description |
|---|---|
| `background` | Solid colour or gradient fill |
| `image` | Bitmap asset from `DbAsset` |
| `text` | Rendered text with font/size/colour/alignment in `SettingsJson` |
| `shape` | Rectangle, circle, or polygon drawn programmatically |
| `overlay` | Semi-transparent colour or pattern on top of other layers |

## Filters

Filters are stored per-layer inside `DbLayer.SettingsJson` as a `filters` array. They are applied during render to the layer's output before compositing.

Planned filter types:

| Filter | Parameters |
|---|---|
| `brightness` | `value` (-100 to +100) |
| `contrast` | `value` (-100 to +100) |
| `saturation` | `value` (-100 to +100) |
| `hue-rotate` | `degrees` (0–360) |
| `blur` | `radius` (pixels) |
| `grayscale` | none |
| `sepia` | `amount` (0–1) |
| `opacity` | `value` (0–1) |

### SettingsJson schema (per layer type)

```json
// image layer example
{
  "x": 0, "y": 0, "width": 800, "height": 600,
  "objectFit": "cover",
  "filters": [
    { "type": "brightness", "value": 10 },
    { "type": "grayscale" }
  ]
}

// text layer example
{
  "x": 50, "y": 50,
  "text": "{{event.title}}",
  "fontFamily": "Inter", "fontSize": 48,
  "color": "#ffffff", "align": "left",
  "filters": []
}
```

Template variables (`{{field}}`) in text layers are resolved at render time from a `variables` dictionary passed to `RenderAsync`.

## IDesignService Methods to Implement

### Template management
```csharp
Task<List<DbTemplate>> GetTemplatesAsync(int? userId = null, CancellationToken ct = default);
Task<DbTemplate?> GetTemplateAsync(int templateId, CancellationToken ct = default);
Task<DbTemplate> CreateTemplateAsync(int userId, string name, int width, int height, CancellationToken ct = default);
Task<DbTemplate> UpdateTemplateAsync(int templateId, string name, string? description, string? tags, string? category, CancellationToken ct = default);
Task DeleteTemplateAsync(int templateId, CancellationToken ct = default);
Task<DbTemplate> PublishTemplateAsync(int templateId, bool published, CancellationToken ct = default);
```

### Layer management
```csharp
Task<DbLayer> AddLayerAsync(int templateId, string layerType, string name, int zIndex, int? assetId, string? settingsJson, CancellationToken ct = default);
Task<DbLayer> UpdateLayerAsync(int layerId, string name, int zIndex, string? settingsJson, CancellationToken ct = default);
Task ReorderLayersAsync(int templateId, List<(int LayerId, int ZIndex)> order, CancellationToken ct = default);
Task DeleteLayerAsync(int layerId, CancellationToken ct = default);
```

### Asset management
```csharp
Task<List<DbAsset>> GetAssetsAsync(int? userId = null, bool includePublic = true, CancellationToken ct = default);
Task<DbAsset> SaveAssetAsync(int userId, string fileName, byte[] imageBytes, string? tags, bool isPublic, CancellationToken ct = default);
Task DeleteAssetAsync(int assetId, CancellationToken ct = default);
```

### Rendering
```csharp
// Synchronous render — returns PNG bytes directly (small templates, previews)
Task<byte[]> RenderAsync(int templateId, Dictionary<string, string>? variables = null, CancellationToken ct = default);

// Async render — queues a DbRenderJob, returns jobId; result retrieved separately
Task<int> EnqueueRenderAsync(int templateId, int userId, Dictionary<string, string>? variables = null, CancellationToken ct = default);
Task<DbRenderJob?> GetRenderJobAsync(int renderJobId, CancellationToken ct = default);
```

## Render Pipeline (RenderAsync)

```
1. Load template + layers (ordered by ZIndex ASC) + asset bytes from DB
2. Create canvas (Width × Height) using Telerik Document Processing (RadFixedDocument / SkiaSharp)
3. For each layer (bottom to top):
   a. Resolve layer type → render to off-screen bitmap
   b. Apply filters from SettingsJson (brightness, blur, etc.)
   c. Composite onto canvas at layer position
4. Substitute {{variables}} in text layers before render
5. Encode canvas as PNG → return bytes
```

## Technology

Image rendering uses **Telerik Document Processing** (already referenced in `SocialMotive.Core.csproj`):
- `Telerik.Documents.Fixed` — PDF/fixed-document pipeline
- `Telerik.Documents.Fixed.FormatProviders.Image.Skia` — PNG export via SkiaSharp
- `Telerik.Documents.ImageUtils` — image manipulation utilities

## Key Files

| File | Purpose |
|---|---|
| `Services/IDesignService.cs` | Service contract |
| `Services/DesignService.cs` | Implementation |
| `Data/Generator/DbTemplate.cs` | Template entity |
| `Data/Generator/DbLayer.cs` | Layer entity (type + SettingsJson) |
| `Data/Generator/DbAsset.cs` | Asset entity (raw PNG bytes) |
| `Data/Generator/DbRenderJob.cs` | Async render job tracking |
| `Controllers/DesignController.cs` | REST API (served by Designer app) |

## SignalR Integration (DesignHub)

During collaborative editing, layer changes are broadcast via `DesignHub` so all clients in a session see real-time updates:

- Save layer → call `IDesignService.UpdateLayerAsync` → controller calls `IHubContext<DesignHub>.Clients.OthersInGroup(sessionId).SendAsync("ReceiveUpdate", payload)`
- Client receives `ReceiveUpdate` → `designHub.js` invokes `dotNetRef.OnReceiveUpdate` → Blazor component refreshes canvas

## Decisions

| Question | Decision | Rationale |
|---|---|---|
| Filter implementation | Post-process per-layer via SkiaSharp `SKColorFilter.CreateColorMatrix()` (brightness/contrast/saturation/grayscale/sepia) and `SKImageFilter.CreateBlur()` | Per-layer offscreen bitmaps give clean compositing; color matrix covers all CSS-equivalent filters in one pass |
| Variable resolver | `Dictionary<string,string>` flat map | Decoupled from event types; callers populate from any source; `{{key}}` replaced via `string.Replace` |
| Async render | `IBackgroundRenderQueue` (bounded `Channel<int>`) + `RenderWorkerService` (`BackgroundService`) in Designer project | Proper ASP.NET Core hosted-service pattern; survives DI scope, handles backpressure, avoids `Task.Run` scope issues |
| Asset storage | PNG bytes in `DbAsset.ImagePng` (DB) for MVP | No extra infrastructure; migrate to file system / blob storage when asset sizes warrant it |
