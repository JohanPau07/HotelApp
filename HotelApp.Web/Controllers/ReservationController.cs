using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Guest;
using HotelApp.BL.Dtos.Reservation;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using static HotelApp.Web.Models.ReservationFormViewModel;

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
      => View(await BuildIndexModelAsync(filter, null));

    private async Task<ReservationIndexViewModel> BuildIndexModelAsync(string filter, string? error)
    {
        var model = new ReservationIndexViewModel { Filter = filter, Error = error };

        var result = await _reservationService.GetAllAsync();
        if (!result.Success)
        {
            model.Error ??= result.Message;
            return model;
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
                Status = StatusNames.GetValueOrDefault(r.Status, "-"),
                StatusCode = r.Status
            };
        }).ToList();

        return model;
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

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string filter = "todas")
    {
        var current = await _reservationService.GetByIdAsync(id);
        if (!current.Success || current.Data is not Reservation reservation)
            return RedirectToAction(nameof(Index), new { filter });

        var result = await _reservationService.UpdateAsync(new UpdateReservationDto
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            GuestId = reservation.GuestId,
            CheckIn = reservation.CheckIn,
            CheckOut = reservation.CheckOut,
            GuestCount = reservation.GuestCount,
            TotalPrice = reservation.TotalPrice,
            Status = 4 // Cancelada
        });

        if (!result.Success)
            return View(nameof(Index), await BuildIndexModelAsync(filter, result.Message));

        return RedirectToAction(nameof(Index), new { filter });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string filter = "todas")
    {
        var result = await _reservationService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
            return View(nameof(Index), await BuildIndexModelAsync(filter, result.Message));

        return RedirectToAction(nameof(Index), new { filter });
    }
    public async Task<IActionResult> Edit(Guid id, string filter = "todas")
    {
        var result = await _reservationService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Reservation reservation || reservation.Status is 3 or 4)
            return RedirectToAction(nameof(Index), new { filter });

        var guest = (await _guestService.GetByIdAsync(reservation.GuestId)).Data as Guest;

        return View(new ReservationEditViewModel
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            CheckIn = reservation.CheckIn,
            CheckOut = reservation.CheckOut,
            GuestCount = reservation.GuestCount,
            GuestName = guest is null ? "-" : $"{guest.FirstName} {guest.LastName}",
            Filter = filter,
            Rooms = await GetRoomOptionsAsync()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReservationEditViewModel model)
    {
        var current = await _reservationService.GetByIdAsync(model.Id);
        if (!current.Success || current.Data is not Reservation reservation)
            return RedirectToAction(nameof(Index), new { filter = model.Filter });

        var roomResult = await _roomService.GetByIdAsync(model.RoomId);
        if (!roomResult.Success || roomResult.Data is not Room room)
            return await ShowEditFormAsync(model, "Selecciona una habitación.");

        var nights = Math.Max(0, model.CheckOut.DayNumber - model.CheckIn.DayNumber);

        var result = await _reservationService.UpdateAsync(new UpdateReservationDto
        {
            Id = reservation.Id,
            RoomId = room.Id,
            GuestId = reservation.GuestId,
            CheckIn = model.CheckIn,
            CheckOut = model.CheckOut,
            GuestCount = model.GuestCount,
            TotalPrice = nights * room.Rate,
            Status = reservation.Status
        });

        if (!result.Success)
            return await ShowEditFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { filter = model.Filter });
    }

    private async Task<IActionResult> ShowEditFormAsync(ReservationEditViewModel model, string error)
    {
        model.Rooms = await GetRoomOptionsAsync();
        model.Error = error;
        return View(model);
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