using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Models;

public class FloorIndexViewModel
{
    public Guid? BlockId { get; set; }
    public string? BlockName { get; set; }
    public string? Error { get; set; }
    public Dictionary<Guid, string> HotelNames { get; set; } = new();
    public Dictionary<Guid, string> BlockNames { get; set; } = new();
    public List<Floor> Items { get; set; } = new();
}

public class FloorFormViewModel
{
    public Guid? Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid? BlockId { get; set; }
    public int Number { get; set; }
    public bool IsActive { get; set; } = true;

    // todos los pisos
    public Guid? ReturnBlockId { get; set; }

    public List<SelectListItem> Hotels { get; set; } = new();
    public List<SelectListItem> Blocks { get; set; } = new();
    public string? Error { get; set; }
}