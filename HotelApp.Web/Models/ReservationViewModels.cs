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

public class RoomOptionViewModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = "";
    public int Capacity { get; set; }
    public decimal Rate { get; set; }
}

public class ReservationFormViewModel
{
    public Guid RoomId { get; set; }
    public DateOnly CheckIn { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly CheckOut { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
    public int GuestCount { get; set; } = 1;

    // Datos del huésped titular
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string DocumentId { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public List<RoomOptionViewModel> Rooms { get; set; } = new();
    public string? Error { get; set; }
}