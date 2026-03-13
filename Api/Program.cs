using DataAccess;
using Serilog;
using Microsoft.OpenApi;
using VehicleBooking.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using VehicleBooking.Models.DTOs;

// Khởi tạo Logger ban đầu
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information(">>> Đang khởi tạo ứng dụng...");
    var builder = WebApplication.CreateBuilder(args);

    // Cấu hình Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());


    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

    // Register layer services
    builder.Services.AddDataAccess(builder.Configuration);
    builder.Services.AddBusinessServices();

    // Cấu hình FluentValidation
    builder.Services.AddFluentValidationAutoValidation(); // Tự động validate khi nhận Request
    builder.Services.AddFluentValidationClientsideAdapters(); // Hỗ trợ validate phía client nếu cần

    // Sử dụng Reflection để tìm tất cả Validators trong Assembly hiện tại
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Gom tất cả các lỗi từ FluentValidation lại thành một chuỗi hoặc chi tiết
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var errorResponse = new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Dữ liệu đầu vào không hợp lệ",
            Details = string.Join("; ", errors) // Nối các lỗi lại thành chuỗi Details
        };

        return new BadRequestObjectResult(errorResponse);
    };
});

    // Cấu hình Swagger
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

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "Booking_"; // Tiền tố cho các Key trong Redis
    });


    var jwtSection = builder.Configuration.GetSection("Jwt");
    var jwtSettings = jwtSection.Get<JwtSettings>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var key = jwtSettings?.Key ?? "A_Very_Long_Temporary_Key_For_EF_Migration_32_Chars";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    var app = builder.Build();

    // Middleware Pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
            c.RoutePrefix = string.Empty; // Để Swagger là trang chủ khi chạy
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information(">>> Ứng dụng đã sẵn sàng! Đang lắng nghe yêu cầu...");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("--------------------------------------");
    Console.WriteLine($"LỖI KHỞI CHẠY: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"LỖI CHI TIẾT (Inner): {ex.InnerException.Message}");
    }
    Console.WriteLine(ex.StackTrace);
    Console.WriteLine("--------------------------------------");
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}