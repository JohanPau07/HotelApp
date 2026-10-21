using System;
using System.Collections.Generic;

namespace HotelApp.DAL.Models;

public partial class Hotel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
