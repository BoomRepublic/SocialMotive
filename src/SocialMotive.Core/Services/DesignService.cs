using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SocialMotive.Core.Data;
using SocialMotive.Core.Data.Generator;
using SocialMotive.Core.Generator;
using SocialMotive.Core.Model.Generator;

namespace SocialMotive.Core.Services;

public class DesignService : IDesignService
{
    private readonly SocialMotiveDbContext _db;
    private readonly ILogger<DesignService> _logger;
    private readonly IBackgroundRenderQueue _queue;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DesignService(SocialMotiveDbContext db, ILogger<DesignService> logger, IBackgroundRenderQueue queue)
    {
        _db = db;
        _logger = logger;
        _queue = queue;
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public async Task<byte[]> RenderAsync(int templateId, Dictionary<string, string>? variables = null, CancellationToken ct = default)
    {
        var template = await _db.Templates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TemplateId == templateId, ct)
            ?? throw new InvalidOperationException($"Template {templateId} not found.");

        var templateData = TemplateJsonHelper.Deserialize(template.TemplateJson);
        var layers = templateData.Layers.OrderBy(l => l.ZIndex).ToList();

        // Pre-load all assets referenced by this template's layers in one query
        var assetIds = layers
            .Where(l => l.LayerType == "image" && !string.IsNullOrEmpty(l.SettingsJson))
            .Select(l => DeserializeSettings<ImageLayerSettings>(l.SettingsJson).AssetId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var assets = assetIds.Any()
            ? await _db.Assets
                .AsNoTracking()
                .Where(a => assetIds.Contains(a.AssetId))
                .ToDictionaryAsync(a => a.AssetId, ct)
            : new Dictionary<int, DbAsset>();

        using var bitmap = new SKBitmap(templateData.Width, templateData.Height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);

        foreach (var layer in layers)
        {
            try
            {
                RenderLayer(canvas, layer, assets, variables, templateData.Width, templateData.Height);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Skipping layer {LayerId} ({LayerType}) due to render error", layer.LayerId, layer.LayerType);
            }
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    public async Task<int> EnqueueRenderAsync(int templateId, int userId, Dictionary<string, string>? variables = null, CancellationToken ct = default)
    {
        var job = new DbRenderJob
        {
            TemplateId = templateId,
            UserId = userId,
            Status = "Queued",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.RenderJobs.Add(job);
        await _db.SaveChangesAsync(ct);

        _queue.Enqueue(job.RenderJobId);

        _logger.LogInformation("Render job {RenderJobId} enqueued for template {TemplateId}", job.RenderJobId, templateId);
        return job.RenderJobId;
    }

    public Task<DbRenderJob?> GetRenderJobAsync(int renderJobId, CancellationToken ct = default)
        => _db.RenderJobs.FindAsync([renderJobId], ct).AsTask();

    // -------------------------------------------------------------------------
    // Layer dispatch
    // -------------------------------------------------------------------------

    private void RenderLayer(SKCanvas canvas, Layer layer, Dictionary<int, DbAsset> assets,
        Dictionary<string, string>? variables, int canvasWidth, int canvasHeight)
    {
        var baseSettings = DeserializeSettings<LayerSettings>(layer.SettingsJson);
        if (!baseSettings.IsVisible) return;

        var dest = GetDestRect(baseSettings.Transform, canvasWidth, canvasHeight);

        switch (layer.LayerType)
        {
            case "background":
                RenderBackground(canvas, baseSettings, dest);
                break;

            case "image":
                var imgSettings = DeserializeSettings<ImageLayerSettings>(layer.SettingsJson);
                if (imgSettings.AssetId.HasValue && assets.TryGetValue(imgSettings.AssetId.Value, out var asset) && asset.ImagePng != null)
                    RenderImage(canvas, imgSettings, asset.ImagePng, dest);
                break;

            case "text":
                var txtSettings = DeserializeSettings<TextLayerSettings>(layer.SettingsJson);
                RenderText(canvas, txtSettings, variables, dest);
                break;

            case "shape":
                var shapeSettings = DeserializeSettings<ShapeLayerSettings>(layer.SettingsJson);
                RenderShape(canvas, shapeSettings, dest);
                break;

            case "overlay":
                RenderOverlay(canvas, baseSettings, dest);
                break;

            default:
                _logger.LogDebug("Unknown layer type '{LayerType}' — skipped", layer.LayerType);
                break;
        }
    }

    // -------------------------------------------------------------------------
    // Layer renderers
    // -------------------------------------------------------------------------

    private static void RenderBackground(SKCanvas canvas, LayerSettings settings, SKRect dest)
    {
        var color = ParseColor(settings.BackgroundColor, SKColors.Black);
        using var paint = new SKPaint { Color = color.WithAlpha(OpacityToByte(settings.Opacity)) };
        canvas.DrawRect(dest, paint);
    }

    private static void RenderImage(SKCanvas canvas, ImageLayerSettings settings, byte[] imageBytes, SKRect dest)
    {
        using var srcBitmap = SKBitmap.Decode(imageBytes);
        if (srcBitmap == null) return;

        var srcRect = new SKRect(0, 0, srcBitmap.Width, srcBitmap.Height);
        var drawRect = ApplyObjectFit(settings.ObjectFit, srcRect, dest);

        using var layerBitmap = new SKBitmap((int)dest.Width, (int)dest.Height);
        using var layerCanvas = new SKCanvas(layerBitmap);
        layerCanvas.Clear(SKColors.Transparent);

        // Translate draw rect relative to the layer's own bitmap origin
        var localRect = new SKRect(
            drawRect.Left - dest.Left,
            drawRect.Top - dest.Top,
            drawRect.Right - dest.Left,
            drawRect.Bottom - dest.Top);

        layerCanvas.DrawBitmap(srcBitmap, localRect);

        using var compositePaint = BuildFilterPaint(settings);
        compositePaint.Color = compositePaint.Color.WithAlpha(OpacityToByte(settings.Opacity));
        canvas.DrawBitmap(layerBitmap, dest.Left, dest.Top, compositePaint);
    }

    private static void RenderText(SKCanvas canvas, TextLayerSettings settings, Dictionary<string, string>? variables, SKRect dest)
    {
        var text = ResolveVariables(settings.TextContent, variables);
        if (string.IsNullOrEmpty(text)) return;

        var color = ParseColor(settings.BackgroundColor, SKColors.White); // BackgroundColor doubles as text color for text layers
        var family = settings.FontFamily ?? "Arial";
        var fontSize = (float)(settings.FontSize ?? 24);
        using var typeface = SKTypeface.FromFamilyName(family, SKFontStyle.Normal) ?? SKTypeface.Default;
        using var font = new SKFont(typeface, fontSize);
        using var paint = new SKPaint
        {
            Color = color.WithAlpha(OpacityToByte(settings.Opacity)),
            IsAntialias = true
        };

        canvas.DrawText(text, dest.Left, dest.Top + font.Size, font, paint);
    }

    private static void RenderShape(SKCanvas canvas, ShapeLayerSettings settings, SKRect dest)
    {
        var fillColor = ParseColor(settings.FillColor ?? settings.BackgroundColor, SKColors.Gray);
        using var fillPaint = new SKPaint
        {
            Color = fillColor.WithAlpha(OpacityToByte(settings.FillOpacity ?? settings.Opacity)),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        switch (settings.ShapeType.ToLowerInvariant())
        {
            case "circle":
            case "ellipse":
                canvas.DrawOval(dest, fillPaint);
                break;
            default: // rectangle
                var radius = (float)(settings.BorderRadiusTL ?? 0);
                if (radius > 0)
                    canvas.DrawRoundRect(dest, radius, radius, fillPaint);
                else
                    canvas.DrawRect(dest, fillPaint);
                break;
        }

        if (settings.StrokeWidth > 0 && settings.StrokeColor != null)
        {
            var strokeColor = ParseColor(settings.StrokeColor, SKColors.Transparent);
            using var strokePaint = new SKPaint
            {
                Color = strokeColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = (float)settings.StrokeWidth,
                IsAntialias = true
            };
            canvas.DrawRect(dest, strokePaint);
        }
    }

    private static void RenderOverlay(SKCanvas canvas, LayerSettings settings, SKRect dest)
    {
        var color = ParseColor(settings.BackgroundColor, SKColors.Black);
        using var paint = new SKPaint
        {
            Color = color.WithAlpha(OpacityToByte(settings.Opacity)),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(dest, paint);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static SKRect GetDestRect(Transform? transform, int canvasWidth, int canvasHeight)
    {
        if (transform == null)
            return new SKRect(0, 0, canvasWidth, canvasHeight);

        var x = (float)(transform.PositionX ?? 0);
        var y = (float)(transform.PositionY ?? 0);
        var w = (float)(transform.Width ?? canvasWidth);
        var h = (float)(transform.Height ?? canvasHeight);

        var zoom = (float)(transform.Zoom ?? 1);
        if (zoom != 1f)
        {
            var cx = x + w / 2f;
            var cy = y + h / 2f;
            w *= zoom;
            h *= zoom;
            x = cx - w / 2f;
            y = cy - h / 2f;
        }

        return new SKRect(x, y, x + w, y + h);
    }

    private static SKRect ApplyObjectFit(string? objectFit, SKRect src, SKRect dest)
    {
        var srcAspect = src.Width / src.Height;
        var destAspect = dest.Width / dest.Height;

        return (objectFit ?? "cover").ToLowerInvariant() switch
        {
            "contain" => srcAspect > destAspect
                ? new SKRect(dest.Left, dest.MidY - dest.Width / srcAspect / 2, dest.Right, dest.MidY + dest.Width / srcAspect / 2)
                : new SKRect(dest.MidX - dest.Height * srcAspect / 2, dest.Top, dest.MidX + dest.Height * srcAspect / 2, dest.Bottom),
            "fill" => dest,
            _ => /* cover */ srcAspect > destAspect
                ? new SKRect(dest.MidX - dest.Height * srcAspect / 2, dest.Top, dest.MidX + dest.Height * srcAspect / 2, dest.Bottom)
                : new SKRect(dest.Left, dest.MidY - dest.Width / srcAspect / 2, dest.Right, dest.MidY + dest.Width / srcAspect / 2)
        };
    }

    /// <summary>Build an SKPaint with color filters derived from ImageLayerSettings filter properties.</summary>
    private static SKPaint BuildFilterPaint(ImageLayerSettings settings)
    {
        var paint = new SKPaint { IsAntialias = true };

        // Collect color matrix filters; blur handled separately as an image filter
        var colorMatrix = BuildColorMatrix(settings);
        SKColorFilter? colorFilter = colorMatrix != null
            ? SKColorFilter.CreateColorMatrix(colorMatrix)
            : null;

        SKImageFilter? imageFilter = null;
        if (settings.FilterBlur is > 0)
        {
            var sigma = (float)settings.FilterBlur / 3f;
            imageFilter = SKImageFilter.CreateBlur(sigma, sigma);
        }

        if (colorFilter != null) paint.ColorFilter = colorFilter;
        if (imageFilter != null) paint.ImageFilter = imageFilter;

        return paint;
    }

    /// <summary>
    /// Returns a 20-element (4×5) color matrix that combines brightness, contrast,
    /// saturation, grayscale, and sepia adjustments, or null if no filters are set.
    /// </summary>
    private static float[]? BuildColorMatrix(ImageLayerSettings s)
    {
        bool hasFilter =
            s.FilterBrightness.HasValue ||
            s.FilterContrast.HasValue ||
            s.FilterSaturation.HasValue ||
            s.FilterHue.HasValue ||
            s.FilterGrayscale.HasValue ||
            s.FilterSepia.HasValue;

        if (!hasFilter) return null;

        // Start with identity matrix
        float[] m = [
            1, 0, 0, 0, 0,
            0, 1, 0, 0, 0,
            0, 0, 1, 0, 0,
            0, 0, 0, 1, 0
        ];

        // Brightness: scale (value 0–200, 100 = normal → scale 0.0–2.0)
        if (s.FilterBrightness is { } brightness && brightness != 100)
        {
            var scale = (float)brightness / 100f;
            m = MultiplyMatrices(m,
            [
                scale, 0, 0, 0, 0,
                0, scale, 0, 0, 0,
                0, 0, scale, 0, 0,
                0, 0, 0, 1, 0
            ]);
        }

        // Contrast: scale around midpoint 0.5 (value 0–200, 100 = normal)
        if (s.FilterContrast is { } contrast && contrast != 100)
        {
            var scale = (float)contrast / 100f;
            var offset = 0.5f * (1f - scale);
            m = MultiplyMatrices(m,
            [
                scale, 0, 0, 0, offset,
                0, scale, 0, 0, offset,
                0, 0, scale, 0, offset,
                0, 0, 0, 1, 0
            ]);
        }

        // Saturation (value 0–200, 100 = normal → sat 0.0–2.0)
        if (s.FilterSaturation is { } saturation && saturation != 100)
        {
            var sat = (float)saturation / 100f;
            const float lr = 0.213f, lg = 0.715f, lb = 0.072f;
            m = MultiplyMatrices(m,
            [
                lr + (1 - lr) * sat, lg - lg * sat, lb - lb * sat, 0, 0,
                lr - lr * sat, lg + (1 - lg) * sat, lb - lb * sat, 0, 0,
                lr - lr * sat, lg - lg * sat, lb + (1 - lb) * sat, 0, 0,
                0, 0, 0, 1, 0
            ]);
        }

        // Hue rotate (value 0–360 degrees)
        if (s.FilterHue is { } hue && hue != 0)
        {
            var rad = (float)(hue * (decimal)Math.PI / 180m);
            var cos = MathF.Cos(rad);
            var sin = MathF.Sin(rad);
            const float lr = 0.213f, lg = 0.715f, lb = 0.072f;
            m = MultiplyMatrices(m,
            [
                lr + cos * (1 - lr) + sin * (-lr),     lg + cos * (-lg) + sin * (-lg),     lb + cos * (-lb) + sin * (1 - lb), 0, 0,
                lr + cos * (-lr) + sin * 0.143f,        lg + cos * (1 - lg) + sin * 0.140f, lb + cos * (-lb) + sin * (-0.283f), 0, 0,
                lr + cos * (-lr) + sin * (-(1 - lr)),   lg + cos * (-lg) + sin * lg,        lb + cos * (1 - lb) + sin * lb,    0, 0,
                0, 0, 0, 1, 0
            ]);
        }

        // Grayscale (value 0–100, 100 = full grayscale)
        if (s.FilterGrayscale is { } grayscale && grayscale > 0)
        {
            var amount = (float)grayscale / 100f;
            const float lr = 0.2126f, lg = 0.7152f, lb = 0.0722f;
            m = MultiplyMatrices(m,
            [
                lr + (1 - lr) * (1 - amount), lg - lg * (1 - amount), lb - lb * (1 - amount), 0, 0,
                lr - lr * (1 - amount), lg + (1 - lg) * (1 - amount), lb - lb * (1 - amount), 0, 0,
                lr - lr * (1 - amount), lg - lg * (1 - amount), lb + (1 - lb) * (1 - amount), 0, 0,
                0, 0, 0, 1, 0
            ]);
        }

        // Sepia (value 0–100, 100 = full sepia)
        if (s.FilterSepia is { } sepia && sepia > 0)
        {
            var amount = (float)sepia / 100f;
            m = MultiplyMatrices(m,
            [
                0.393f * amount + (1 - amount), 0.769f * amount, 0.189f * amount, 0, 0,
                0.349f * amount, 0.686f * amount + (1 - amount), 0.168f * amount, 0, 0,
                0.272f * amount, 0.534f * amount, 0.131f * amount + (1 - amount), 0, 0,
                0, 0, 0, 1, 0
            ]);
        }

        return m;
    }

    /// <summary>Multiply two 4×5 color matrices (row-major).</summary>
    private static float[] MultiplyMatrices(float[] a, float[] b)
    {
        var result = new float[20];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                float sum = col == 4 ? a[row * 5 + 4] : 0;
                for (int k = 0; k < 4; k++)
                    sum += a[row * 5 + k] * b[k * 5 + col];
                result[row * 5 + col] = sum;
            }
        }
        return result;
    }

    private static T DeserializeSettings<T>(string? json) where T : LayerSettings, new()
        => json == null ? new T() : JsonSerializer.Deserialize<T>(json, _jsonOptions) ?? new T();

    private static string ResolveVariables(string? text, Dictionary<string, string>? vars)
    {
        if (vars == null || text == null) return text ?? string.Empty;
        return vars.Aggregate(text, (t, kv) => t.Replace($"{{{{{kv.Key}}}}}", kv.Value));
    }

    private static SKColor ParseColor(string? hex, SKColor fallback)
        => hex != null && SKColor.TryParse(hex, out var color) ? color : fallback;

    private static byte OpacityToByte(decimal opacity)
        => (byte)Math.Clamp((double)(opacity * 255), 0, 255);
}
