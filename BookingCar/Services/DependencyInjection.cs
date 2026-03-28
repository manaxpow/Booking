using Microsoft.Extensions.DependencyInjection;
using Services.Schedule;
using Services.Ticket;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ISeatService, SeatService>();
        services.AddScoped<IDestinationService, DestinationService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IEmailService, EmailService>();


        // Worker
        services.AddHostedService<ExpiredBookingWorker>();

        return services;
    }
}