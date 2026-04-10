// ============================================================
// Bài tập 6: Branching Pipeline với UseWhen
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Pipeline ghi log chi tiết cho internal API (có header X-Internal-Key)
//           và pipeline chuẩn cho public API
//   - UseWhen: branch quay về pipeline chính sau khi xong
//   - MapWhen: branch TỘC LẬP, không quay về
// ============================================================

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

// === UseWhen Branching Pipeline ===
// Request có header "X-Internal-Key" → chạy thêm logging middleware chi tiết
// Sau đó quay về pipeline chính (khác với MapWhen)
app.UseWhen(
    context => context.Request.Headers.ContainsKey("X-Internal-Key"),
    appBuilder =>
    {
        appBuilder.Use(async (context, next) =>
        {
            var key = context.Request.Headers["X-Internal-Key"].ToString();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            logger.LogInformation(
                "[Internal API] Request từ {IP} với key {Key}",
                context.Connection.RemoteIpAddress,
                key.Length >= 4 ? key[..4] + "..." : key);

            // Ghi log body request
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            logger.LogInformation("[Internal API] Body: {Body}", body);

            await next();

            logger.LogInformation(
                "[Internal API] Response {StatusCode}",
                context.Response.StatusCode);
        });
    }
);

// Pipeline chính — luôn chạy
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("[Pipeline] {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Endpoint demo
app.MapGet("/api/public", () => new { Message = "Public API endpoint" });
app.MapGet("/api/internal", () => new { Message = "Internal API endpoint" });
app.MapPost("/api/data", (DataRequest request) => new { Received = request.Value });

app.Run();

public record DataRequest(string Value);
