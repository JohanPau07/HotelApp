namespace HotelApp.DAL.Models.Base;

public abstract class FloorBase
{
    public Guid HotelId { get; set; }
    public int Number { get; set; }
    public bool IsActive { get; set; } = true;
}