using DataAccess;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VehicleBooking.Api.Extensions;
using VehicleBooking.Api.Middlewares;

// 1. Bootstrap Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 2. Logging
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // 3. Extensions (Services)
    builder.Services.AddIdentityServices(builder.Configuration);
    builder.Services.AddCacheExtensions(builder.Configuration);
    builder.Services.AddValidationServices();
    builder.Services.AddSwaggerDocumentation();

    // Core Layers
    builder.Services.AddDataAccess(builder.Configuration);
    builder.Services.AddBusinessServices();

    builder.Services.Configure<SePaySettings>(builder.Configuration.GetSection(SePaySettings.SectionName));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

    builder.Services.AddControllers();

    var app = builder.Build();

    // 4. Pipeline (Middlewares)
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseSwaggerDocumentation();


    app.UseAuthentication();
    app.UseAuthorization();

    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.Headers.ContainsKey("Age"))
                context.Response.Headers["X-Cache-Status"] = "HIT";
            else
                context.Response.Headers["X-Cache-Status"] = "MISS";
            return Task.CompletedTask;
        });
        await next();
    });
    app.UseOutputCache();

    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync(); // Apply pending migrations
                await AppDbContextSeed.SeedData(context); // Seed data
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }

    Log.Information(">>> Ứng dụng đã sẵn sàng!");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}