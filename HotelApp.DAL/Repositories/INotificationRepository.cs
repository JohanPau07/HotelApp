using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Notification;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<OperationResult> AddAsync(NotificationAdd model);
    Task<OperationResult> UpdateAsync(NotificationUpdate model);
}