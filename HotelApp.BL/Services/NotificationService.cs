using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Notification;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Notification;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class NotificationService : BaseService<Notification, SaveNotificationDto, UpdateNotificationDto>, INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository repository) : base(repository)
    {
        _notificationRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveNotificationDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _notificationRepository.AddAsync(new NotificationAdd
        {
            ReservationId = dto.ReservationId,
            Type = dto.Type,
            Message = dto.Message.Trim(),
            Status = dto.Status,
            SentAt = dto.SentAt,
            FailureReason = dto.FailureReason?.Trim()
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateNotificationDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _notificationRepository.UpdateAsync(new NotificationUpdate
        {
            Id = dto.Id,
            ReservationId = dto.ReservationId,
            Type = dto.Type,
            Message = dto.Message.Trim(),
            Status = dto.Status,
            SentAt = dto.SentAt,
            FailureReason = dto.FailureReason?.Trim()
        });
    }

    private static OperationResult? Validate(NotificationBase dto)
    {
        if (dto.ReservationId == Guid.Empty)
            return OperationResult.Fail("La reserva de la notificación es obligatoria.");
        if (dto.Type > 2)
            return OperationResult.Fail("El tipo de notificación no es válido.");
        if (string.IsNullOrWhiteSpace(dto.Message))
            return OperationResult.Fail("El mensaje de la notificación es obligatorio.");
        if (dto.Status > 2)
            return OperationResult.Fail("El estado de la notificación no es válido.");
        return null;
    }
}