using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Models;

public class RoomIndexViewModel
{
    public Guid? FloorId { get; set; }
    public string? FloorLabel { get; set; }
    public string? Error { get; set; }
    public Dictionary<Guid, string> FloorLabels { get; set; } = new();
    public Dictionary<Guid, string> RoomTypeNames { get; set; } = new();
    public Dictionary<byte, string> StatusNames { get; set; } = new();
    public List<Room> Items { get; set; } = new();
}

public class RoomTypeOptionViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal BaseRate { get; set; }
    public int Capacity { get; set; }
}

public class RoomFormViewModel
{
    public Guid? Id { get; set; }
    public Guid FloorId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Number { get; set; } = "";
    public int Capacity { get; set; } = 1;
    public decimal Rate { get; set; }
    public byte Status { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    // todas las habitaciones
    public Guid? ReturnFloorId { get; set; }

    public List<SelectListItem> Floors { get; set; } = new();
    public List<RoomTypeOptionViewModel> RoomTypes { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();
    public string? Error { get; set; }
}