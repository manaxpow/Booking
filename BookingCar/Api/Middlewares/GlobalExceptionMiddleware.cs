using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleBooking.Models.DTOs;

namespace VehicleBooking.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi vào Console/File/Database
            _logger.LogError(ex, "Một lỗi không mong đợi đã xảy ra: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mặc định là lỗi 500
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Lỗi hệ thống nội bộ.";
       
        // Xử lý riêng các loại lỗi cụ thể (Fundamentals)
        if (exception is DbUpdateConcurrencyException)
        {

            statusCode = (int)HttpStatusCode.Conflict;
            message = "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại trang (Race Condition).";
        }

        if (exception is DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
        }
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = _env.IsDevelopment() ? exception.StackTrace : null
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Console.WriteLine(exception.ToString());
        await context.Response.WriteAsync(json);
    }
}