using Microsoft.OpenApi;
using Serilog;
using VehicleBooking.Api.Middlewares;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Vehicle Booking API",
            Version = "v1",
            Description = "Hệ thống đặt xe trực tuyến"
        });

        var apiXml = Path.Combine(AppContext.BaseDirectory, "VehicleBooking.Api.xml");
        var modelsXml = Path.Combine(AppContext.BaseDirectory, "VehicleBooking.Models.xml");

        if (File.Exists(apiXml)) options.IncludeXmlComments(apiXml);
        if (File.Exists(modelsXml)) options.IncludeXmlComments(modelsXml);
    });

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddSingleton<IUserStore, UserStore>();

    Log.Information(">>> Ứng dụng đã sẵn sàng! Đang lắng nghe kết nối...");
    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();


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