using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<IDestinationRepository, DestinationRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        return services;
    }
}