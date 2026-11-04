using HotelApp.BL.Interfaces;
using HotelApp.BL.Services;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL.Dependencies;

public static class NotificationDependency
{
    public static IServiceCollection AddNotificationDependency(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}