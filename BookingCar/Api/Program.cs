using DataAccess;
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
    builder.Services.AddValidationServices();
    builder.Services.AddSwaggerDocumentation();

    // Core Layers
    builder.Services.AddDataAccess(builder.Configuration);
    builder.Services.AddBusinessServices();

    builder.Services.Configure<SePaySettings>(builder.Configuration.GetSection(SePaySettings.SectionName));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "Booking_";
    });

    builder.Services.AddControllers();

    var app = builder.Build();

    // 4. Pipeline (Middlewares)
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseSwaggerDocumentation();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

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