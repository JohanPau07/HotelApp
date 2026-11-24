using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using HotelApp.BL.Dtos.Guest;
using HotelApp.BL.Dtos.Reservation;

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
    public async Task<IActionResult> Create()
    {
        var model = new ReservationFormViewModel { Rooms = await GetRoomOptionsAsync() };
        return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationFormViewModel model)
    {
        
        ModelState.Clear();

        var roomResult = await _roomService.GetByIdAsync(model.RoomId);
        if (!roomResult.Success || roomResult.Data is not Room room)
            return await ShowFormAsync(model, "Selecciona una habitación.");

        // Huésped titular: si el documento ya existe se reutiliza, si no se crea
        var documentId = (model.DocumentId ?? "").Trim();
        var guests = (await _guestService.GetAllAsync()).Data as List<Guest> ?? new List<Guest>();
        var guest = guests.FirstOrDefault(g => g.DocumentId == documentId);

        if (guest is null)
        {
            var guestResult = await _guestService.SaveAsync(new SaveGuestDto
            {
                FirstName = model.FirstName ?? "",
                LastName = model.LastName ?? "",
                DocumentId = documentId,
                Email = model.Email,
                Phone = model.Phone
            });

            if (!guestResult.Success || guestResult.Data is not Guest newGuest)
                return await ShowFormAsync(model, guestResult.Message);

            guest = newGuest;
        }

        var nights = Math.Max(0, model.CheckOut.DayNumber - model.CheckIn.DayNumber);

        var result = await _reservationService.SaveAsync(new SaveReservationDto
        {
            RoomId = room.Id,
            GuestId = guest.Id,
            CheckIn = model.CheckIn,
            CheckOut = model.CheckOut,
            GuestCount = model.GuestCount,
            TotalPrice = nights * room.Rate,
            Status = 1 // Confirmada
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { filter = "nuevas" });
    }

    private async Task<IActionResult> ShowFormAsync(ReservationFormViewModel model, string error)
    {
        model.Rooms = await GetRoomOptionsAsync();
        model.Error = error;
        return View(model);
    }
    private async Task<List<RoomOptionViewModel>> GetRoomOptionsAsync()
    {
        var hotels = ((await _hotelService.GetAllAsync()).Data as List<Hotel> ?? new List<Hotel>())
            .ToDictionary(h => h.Id, h => h.Name);
        var floors = ((await _floorService.GetAllAsync()).Data as List<Floor> ?? new List<Floor>())
            .ToDictionary(f => f.Id, f => f);
        var rooms = (await _roomService.GetAllAsync()).Data as List<Room> ?? new List<Room>();

        return rooms
            .Where(r => r.IsActive && r.Status != 4 && r.Status != 5 && floors.ContainsKey(r.FloorId))
            .Select(r =>
            {
                var floor = floors[r.FloorId];
                var hotel = hotels.GetValueOrDefault(floor.HotelId, "-");
                return new RoomOptionViewModel
                {
                    Id = r.Id,
                    Label = $"{hotel} - Piso {floor.Number} - Hab. {r.Number} (máx. {r.Capacity} personas)",
                    Capacity = r.Capacity,
                    Rate = r.Rate
                };
            })
            .OrderBy(o => o.Label)
            .ToList();
    }
}