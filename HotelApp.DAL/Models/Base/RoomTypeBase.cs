namespace HotelApp.DAL.Models.Base;

public abstract class RoomTypeBase
{
    public string Name { get; set; } = null!;
    public decimal BaseRate { get; set; }
    public int Capacity { get; set; } = 1;
}