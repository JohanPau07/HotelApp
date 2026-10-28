namespace HotelApp.DAL.Models.Base;

public abstract class RoomBase
{
    public Guid FloorId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Number { get; set; } = null!;
    public int Capacity { get; set; }
    public decimal Rate { get; set; }
    public byte Status { get; set; }
    public bool IsActive { get; set; } = true;
}