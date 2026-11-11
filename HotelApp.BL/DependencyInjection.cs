using Microsoft.Extensions.DependencyInjection;
using HotelApp.BL.Dependencies;

namespace HotelApp.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBL(this IServiceCollection services)
        {
            services
                .AddHotelDependency()
                .AddBlockDependency()
                .AddFloorDependency()
                .AddRoomDependency()
                .AddGuestDependency()
                .AddNotificationDependency()
                .AddReservationDependency();
            return services;
        }
    }
}