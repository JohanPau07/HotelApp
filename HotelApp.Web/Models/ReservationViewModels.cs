namespace HotelApp.Web.Models;

public class ReservationRowViewModel
{
    public Guid Id { get; set; }
    public string Hotel { get; set; } = "-";
    public string Room { get; set; } = "-";
    public string Guest { get; set; } = "-";
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "";
}

public class ReservationIndexViewModel
{
    public string Filter { get; set; } = "todas";
    public string? Error { get; set; }
    public List<ReservationRowViewModel> Items { get; set; } = new();
}