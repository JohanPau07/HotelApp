using HotelApp.BL.Interfaces;
using HotelApp.BL.Services;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL.Dependencies;

public static class ReservationDependency
{
    public static IServiceCollection AddReservationDependency(this IServiceCollection services)
    {
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationService, ReservationService>();
        return services;
    }
}