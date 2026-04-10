// ============================================================
using System.Diagnostics;

// Bài tập 1: Custom Middleware Logging Thời Gian
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Viết middleware đo thời gian xử lý request, thêm header X-Response-Time
// ============================================================

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Đăng ký TimingMiddleware
app.UseMiddleware<TimingMiddleware>();

// Endpoint test
app.MapGet("/api/product", () => new
{
    Id = 1,
    Name = "Laptop Gaming",
    Price = 2500.00m,
    Category = "Điện tử"
});

app.MapGet("/api/slow", async () =>
{
    await Task.Delay(200); // Giả lập xử lý chậm
    return new { Message = "Xử lý xong sau 200ms" };
});

app.MapGet("/api/fast", () => Results.Ok(new { Message = "Phản hồi nhanh!" }));

app.Run();

// ----------------------------------------------------------
// TimingMiddleware: Đo thời gian xử lý request
// ----------------------------------------------------------
public class TimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TimingMiddleware> _logger;

    public TimingMiddleware(RequestDelegate next, ILogger<TimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        // Gọi middleware tiếp theo trong pipeline
        await _next(context);

        sw.Stop();

        // Thêm header X-Response-Time
        context.Response.Headers["X-Response-Time"] = $"{sw.ElapsedMilliseconds}ms";

        // Log
        _logger.LogInformation(
            "[Timing] {Method} {Path} -> {StatusCode} ({ElapsedMs}ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}
