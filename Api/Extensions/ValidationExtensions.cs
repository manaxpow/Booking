using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using VehicleBooking.Models.DTOs;

namespace VehicleBooking.Api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                var errorResponse = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Dữ liệu đầu vào không hợp lệ",
                    Details = string.Join("; ", errors)
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });

        return services;
    }
}