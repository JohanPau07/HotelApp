using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class RoomType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal BaseRate { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
