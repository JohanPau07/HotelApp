using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

public class ReservationController : Controller
{
    private static readonly Dictionary<byte, string> StatusNames = new()
    {
        [0] = "Pendiente",
        [1] = "Confirmada",
        [2] = "En curso",
        [3] = "Completada",
        [4] = "Cancelada"
    };

    private readonly IReservationService _reservationService;
    private readonly IGuestService _guestService;
    private readonly IRoomService _roomService;
    private readonly IFloorService _floorService;
    private readonly IHotelService _hotelService;

    public ReservationController(IReservationService reservationService, IGuestService guestService,
        IRoomService roomService, IFloorService floorService, IHotelService hotelService)
    {
        _reservationService = reservationService;
        _guestService = guestService;
        _roomService = roomService;
        _floorService = floorService;
        _hotelService = hotelService;
    }

    public async Task<IActionResult> Index(string filter = "todas")
    {
        var model = new ReservationIndexViewModel { Filter = filter };

        var result = await _reservationService.GetAllAsync();
        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        var reservations = result.Data as List<Reservation> ?? new List<Reservation>();
        var today = DateOnly.FromDateTime(DateTime.Today);

        reservations = filter switch
        {
            "proximas" => reservations
                .Where(r => (r.Status == 1 || r.Status == 2) && r.CheckOut >= today && r.CheckOut <= today.AddDays(3))
                .OrderBy(r => r.CheckOut).ToList(),
            "nuevas" => reservations
                .Where(r => r.CreatedAt >= DateTime.Now.AddDays(-7))
                .OrderByDescending(r => r.CreatedAt).ToList(),
            _ => reservations.OrderByDescending(r => r.CreatedAt).ToList()
        };

        var guests = ((await _guestService.GetAllAsync()).Data as List<Guest> ?? new List<Guest>())
            .ToDictionary(g => g.Id, g => $"{g.FirstName} {g.LastName}");
        var hotels = ((await _hotelService.GetAllAsync()).Data as List<Hotel> ?? new List<Hotel>())
            .ToDictionary(h => h.Id, h => h.Name);
        var floors = ((await _floorService.GetAllAsync()).Data as List<Floor> ?? new List<Floor>())
            .ToDictionary(f => f.Id, f => f.HotelId);
        var rooms = ((await _roomService.GetAllAsync()).Data as List<Room> ?? new List<Room>())
            .ToDictionary(r => r.Id, r => r);

        model.Items = reservations.Select(r =>
        {
            rooms.TryGetValue(r.RoomId, out var room);
            var hotelName = room != null
                            && floors.TryGetValue(room.FloorId, out var hotelId)
                            && hotels.TryGetValue(hotelId, out var name)
                ? name
                : "-";

            return new ReservationRowViewModel
            {
                Id = r.Id,
                Hotel = hotelName,
                Room = room?.Number ?? "-",
                Guest = guests.GetValueOrDefault(r.GuestId, "-"),
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                GuestCount = r.GuestCount,
                TotalPrice = r.TotalPrice,
                Status = StatusNames.GetValueOrDefault(r.Status, "-")
            };
        }).ToList();

        return View(model);
    }
}