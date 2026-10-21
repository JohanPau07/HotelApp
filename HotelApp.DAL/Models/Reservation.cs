using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class Reservation
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid GuestId { get; set; }

    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int GuestCount { get; set; }

    public decimal TotalPrice { get; set; }

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Room Room { get; set; } = null!;
}
