using HotelApp.DAL.Models;

namespace HotelApp.Web.Models;

public class BlockIndexViewModel
{
    public Guid? HotelId { get; set; }
    public string? HotelName { get; set; }
    public string? Error { get; set; }
    public Dictionary<Guid, string> HotelNames { get; set; } = new();
    public List<Block> Items { get; set; } = new();
}