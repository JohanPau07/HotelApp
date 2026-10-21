using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services)
        {
            // Aquí se registrarán el DbContext y los repositorios
            return services;
        }
    }
}