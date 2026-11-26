using HotelApp.DAL.Models;

namespace HotelApp.Web.Models;

public class HotelIndexViewModel
{
    public string? Error { get; set; }
    public List<Hotel> Items { get; set; } = new();
}

public class HotelFormViewModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public byte Stars { get; set; } = 1;
    public string? Description { get; set; }
    public string? Services { get; set; }
    public string? Error { get; set; }
}