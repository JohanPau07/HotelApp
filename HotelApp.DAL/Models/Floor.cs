using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class Floor
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public int Number { get; set; }

    public bool IsActive { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
