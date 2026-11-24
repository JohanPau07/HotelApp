namespace HotelApp.DAL.Models.Base;

public abstract class HotelBase
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public byte Stars { get; set; } = 1;

    public string? Description { get; set; }

    public string? Services { get; set; }
}