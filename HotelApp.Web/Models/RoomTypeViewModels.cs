using HotelApp.DAL.Models;

namespace HotelApp.Web.Models;

public class RoomTypeIndexViewModel
{
    public string? Error { get; set; }
    public List<RoomType> Items { get; set; } = new();
}

public class RoomTypeFormViewModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "";
    public decimal BaseRate { get; set; }
    public int Capacity { get; set; } = 1;
    public string? Error { get; set; }
}