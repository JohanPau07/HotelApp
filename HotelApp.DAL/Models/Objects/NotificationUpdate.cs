using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Notification;

public sealed class NotificationUpdate : NotificationBase
{
    public Guid Id { get; set; }
}