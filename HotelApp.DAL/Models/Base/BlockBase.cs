namespace HotelApp.DAL.Models.Base;

public abstract class BlockBase
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}