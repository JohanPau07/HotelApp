using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Models;

public class BlockIndexViewModel
{
    public Guid? HotelId { get; set; }
    public string? HotelName { get; set; }
    public string? Error { get; set; }
    public Dictionary<Guid, string> HotelNames { get; set; } = new();
    public List<Block> Items { get; set; } = new();
}
public class BlockFormViewModel
{
    public Guid? Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;

    // todos los bloques
    public Guid? ReturnHotelId { get; set; }

    public List<SelectListItem> Hotels { get; set; } = new();
    public string? Error { get; set; }
}