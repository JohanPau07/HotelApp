using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid ReservationId { get; set; }

    public byte Type { get; set; }

    public string Message { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public string? FailureReason { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
}
