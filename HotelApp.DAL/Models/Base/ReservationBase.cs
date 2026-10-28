namespace HotelApp.DAL.Models.Base;

public abstract class ReservationBase
{
    public Guid RoomId { get; set; }
    public Guid GuestId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public byte Status { get; set; }
}