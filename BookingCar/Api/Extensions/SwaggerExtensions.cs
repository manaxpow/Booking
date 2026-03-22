using Microsoft.OpenApi.Models;

namespace VehicleBooking.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vehicle Booking API",
                Version = "v1",
                Description = "Hệ thống đặt xe trực tuyến"
            });

            // 1. Định nghĩa Security Scheme
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });

            // 2. Áp dụng Security Requirement
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // 3. XML Comments
            var apiXml = Path.Combine(AppContext.BaseDirectory, "VehicleBooking.Api.xml");
            var modelsXml = Path.Combine(AppContext.BaseDirectory, "VehicleBooking.Models.xml");

            if (File.Exists(apiXml)) options.IncludeXmlComments(apiXml);
            if (File.Exists(modelsXml)) options.IncludeXmlComments(modelsXml);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
                c.RoutePrefix = string.Empty; // Swagger là trang chủ
            });
        }
        return app;
    }
}