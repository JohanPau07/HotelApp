using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBL(this IServiceCollection services)
        {
            // Aquí se registrarán los servicios de negocio
            return services;
        }
    }
}