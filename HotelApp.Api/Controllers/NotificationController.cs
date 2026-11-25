using HotelApp.BL.Dtos.Notification;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class NotificationController : BaseApiController<SaveNotificationDto, UpdateNotificationDto>
{
    public NotificationController(INotificationService service) : base(service) { }
}