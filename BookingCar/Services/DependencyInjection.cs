using Microsoft.Extensions.DependencyInjection;
using Services.Schedule;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ISeatService, SeatService>();
        services.AddScoped<IDestinationService, DestinationService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}