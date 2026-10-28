using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Notification;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(NotificationAdd model)
    {
        // CreatedAt y UpdatedAt los pone la base de datos por defecto
        var notification = new Notification
        {
            ReservationId = model.ReservationId,
            Type = model.Type,
            Message = model.Message,
            Status = model.Status,
            SentAt = model.SentAt,
            FailureReason = model.FailureReason
        };
        _context.Notifications.Add(notification);
        return await SaveAsync("Notificación creada", notification);
    }

    public async Task<OperationResult> UpdateAsync(NotificationUpdate model)
    {
        var notification = await _context.Notifications.FindAsync(model.Id);
        if (notification is null) return OperationResult.Fail("Notificación no encontrada");

        notification.ReservationId = model.ReservationId;
        notification.Type = model.Type;
        notification.Message = model.Message;
        notification.Status = model.Status;
        notification.SentAt = model.SentAt;
        notification.FailureReason = model.FailureReason;
        notification.UpdatedAt = DateTime.Now;
        return await SaveAsync("Notificación actualizada", notification);
    }
}