using HotelApp.BL.Interfaces;
using HotelApp.BL.Services;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL.Dependencies;

public static class FloorDependency
{
    public static IServiceCollection AddFloorDependency(this IServiceCollection services)
    {
        services.AddScoped<IFloorRepository, FloorRepository>();
        services.AddScoped<IFloorService, FloorService>();
        return services;
    }
}