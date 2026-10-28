namespace HotelApp.DAL.Models.Base;

public abstract class NotificationBase
{
    public Guid ReservationId { get; set; }
    public byte Type { get; set; }
    public string Message { get; set; } = null!;
    public byte Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
}