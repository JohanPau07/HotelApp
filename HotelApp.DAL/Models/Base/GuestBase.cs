namespace HotelApp.DAL.Models.Base;

public abstract class GuestBase
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string DocumentId { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}