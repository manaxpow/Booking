// ============================================================
// Bài tập 8: IOptionsMonitor — Cấu hình Reload Realtime
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Tạo service đọc CacheSettings qua IOptionsMonitor,
//           subscribe sự thay đổi config không cần restart
//   - IOptions<T>: singleton, không đổi sau startup
//   - IOptionsSnapshot<T>: scoped, đổi mỗi request
//   - IOptionsMonitor<T>: singleton, nhận notification khi config thay đổi
// ============================================================

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Bind CacheSettings từ appsettings.json
builder.Services.Configure<CacheSettings>(
    builder.Configuration.GetSection("CacheSettings"));

// Register demo service
builder.Services.AddSingleton<CacheConfigService>();

var app = builder.Build();

app.MapGet("/api/cache/config", (CacheConfigService service) =>
{
    var current = service.GetCurrent();
    return Results.Ok(new
    {
        ttlMinutes = current.TtlMinutes,
        maxItems = current.MaxItems,
        note = "Thay đổi appsettings.json → config tự reload (IOptionsMonitor)",
        testSteps = new[]
        {
            "1. Gọi endpoint này → xem config hiện tại",
            "2. Sửa appsettings.json: CacheSettings.TtlMinutes = 99",
            "3. Lưu file → gọi lại endpoint → thấy giá trị mới!"
        }
    });
});

app.MapGet("/api/cache/config/history", (CacheConfigService service) =>
{
    return Results.Ok(new
    {
        changeHistory = service.GetChangeHistory(),
        explanation = "Mỗi lần thay đổi appsettings.json, IOptionsMonitor trigger OnChange callback"
    });
});

app.MapGet("/api/cache/options-comparison", (IOptions<CacheSettings> options,
    IOptionsSnapshot<CacheSettings> snapshot,
    IOptionsMonitor<CacheSettings> monitor) =>
{
    return Results.Ok(new
    {
        iOptions = new
        {
            description = "IOptions<T> — Singleton, đọc 1 lần lúc startup, KHÔNG thay đổi",
            ttlMinutes = options.Value.TtlMinutes,
            maxItems = options.Value.MaxItems
        },
        iOptionsSnapshot = new
        {
            description = "IOptionsSnapshot<T> — Scoped, đọc mỗi request, thay đổi khi config reload",
            ttlMinutes = snapshot.Value.TtlMinutes,
            maxItems = snapshot.Value.MaxItems
        },
        iOptionsMonitor = new
        {
            description = "IOptionsMonitor<T> — Singleton, nhận notification khi config thay đổi",
            ttlMinutes = monitor.CurrentValue.TtlMinutes,
            maxItems = monitor.CurrentValue.MaxItems
        },
        whenToUse = new
        {
            IOptions = "Khi config KHÔNG thay đổi lúc runtime",
            IOptionsSnapshot = "Khi cần config mới mỗi request (scoped dependencies)",
            IOptionsMonitor = "Khi cần react to config changes realtime (recommended cho settings)"
        }
    });
});

app.Run();

// ----------------------------------------------------------
// CacheSettings
// ----------------------------------------------------------
public class CacheSettings
{
    public int TtlMinutes { get; set; } = 5;
    public int MaxItems { get; set; } = 1000;
}

// ----------------------------------------------------------
// CacheConfigService — subscribe config changes
// ----------------------------------------------------------
public class CacheConfigService
{
    private CacheSettings _current;
    private readonly List<string> _changeHistory = [];
    private readonly ILogger<CacheConfigService> _logger;

    public CacheConfigService(IOptionsMonitor<CacheSettings> monitor,
        ILogger<CacheConfigService> logger)
    {
        _current = monitor.CurrentValue;
        _logger = logger;

        // Subscribe to config changes
        monitor.OnChange(newSettings =>
        {
            var oldTtl = _current.TtlMinutes;
            var oldMax = _current.MaxItems;

            _logger.LogInformation(
                "Cache config changed: TTL {OldTtl}->{NewTtl}, MaxItems {OldMax}->{NewMax}",
                oldTtl, newSettings.TtlMinutes,
                oldMax, newSettings.MaxItems);

            _changeHistory.Add($"[{DateTime.Now:HH:mm:ss}] TTL: {oldTtl} → {newSettings.TtlMinutes}, MaxItems: {oldMax} → {newSettings.MaxItems}");
            _current = newSettings;
        });

        _changeHistory.Add($"[{DateTime.Now:HH:mm:ss}] Initial: TTL={_current.TtlMinutes}, MaxItems={_current.MaxItems}");
    }

    public CacheSettings GetCurrent() => _current;

    public List<string> GetChangeHistory() => _changeHistory;
}
