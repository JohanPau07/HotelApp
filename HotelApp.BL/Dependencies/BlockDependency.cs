using HotelApp.BL.Interfaces;
using HotelApp.BL.Services;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL.Dependencies;

public static class BlockDependency
{
    public static IServiceCollection AddBlockDependency(this IServiceCollection services)
    {
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<IBlockService, BlockService>();
        return services;
    }
}