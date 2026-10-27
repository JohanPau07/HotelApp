namespace HotelApp.DAL.Models.Base;

public abstract class HotelBase
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}