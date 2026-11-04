using HotelApp.BL.Interfaces;
using HotelApp.BL.Services;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL.Dependencies;

public static class GuestDependency
{
    public static IServiceCollection AddGuestDependency(this IServiceCollection services)
    {
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<IGuestService, GuestService>();
        return services;
    }
}