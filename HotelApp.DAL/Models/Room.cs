using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class Room
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public Guid RoomTypeId { get; set; }

    public string Number { get; set; } = null!;

    public int Capacity { get; set; }

    public decimal Rate { get; set; }

    public byte Status { get; set; }

    public bool IsActive { get; set; }

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual RoomType RoomType { get; set; } = null!;
}
