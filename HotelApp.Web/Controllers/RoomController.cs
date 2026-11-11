using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Room;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Controllers;

public class RoomController : Controller
{
    private static readonly Dictionary<byte, string> StatusNames = new()
    {
        [0] = "Disponible",
        [1] = "Reservada",
        [2] = "Ocupada",
        [3] = "Limpieza",
        [4] = "Mantenimiento",
        [5] = "Fuera de servicio"
    };

    private readonly IRoomService _roomService;
    private readonly IFloorService _floorService;
    private readonly IRoomTypeService _roomTypeService;
    private readonly IHotelService _hotelService;

    public RoomController(IRoomService roomService, IFloorService floorService,
        IRoomTypeService roomTypeService, IHotelService hotelService)
    {
        _roomService = roomService;
        _floorService = floorService;
        _roomTypeService = roomTypeService;
        _hotelService = hotelService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _roomService.GetAllAsync();
        if (!result.Success)
            TempData["Error"] = result.Message;

        ViewBag.FloorNames = (await GetFloorLabelsAsync());
        ViewBag.RoomTypeNames = (await GetRoomTypesAsync()).ToDictionary(t => t.Id, t => t.Name);
        ViewBag.StatusNames = StatusNames;
        return View(result.Data as List<Room> ?? new List<Room>());
    }

    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        return View(new SaveRoomDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveRoomDto dto)
    {
        ModelState.Remove(nameof(dto.FloorId));
        ModelState.Remove(nameof(dto.RoomTypeId));

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(dto);
        }

        var result = await _roomService.SaveAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadListsAsync();
            return View(dto);
        }

        TempData["Ok"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _roomService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Room room)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        await LoadListsAsync();
        return View(new UpdateRoomDto
        {
            Id = room.Id,
            FloorId = room.FloorId,
            RoomTypeId = room.RoomTypeId,
            Number = room.Number,
            Capacity = room.Capacity,
            Rate = room.Rate,
            Status = room.Status,
            IsActive = room.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRoomDto dto)
    {
        ModelState.Remove(nameof(dto.FloorId));
        ModelState.Remove(nameof(dto.RoomTypeId));

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(dto);
        }

        var result = await _roomService.UpdateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadListsAsync();
            return View(dto);
        }

        TempData["Ok"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomService.RemoveAsync(new RemoveDto { Id = id });
        TempData[result.Success ? "Ok" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<RoomType>> GetRoomTypesAsync()
    {
        var result = await _roomTypeService.GetAllAsync();
        return result.Data as List<RoomType> ?? new List<RoomType>();
    }

    private async Task<Dictionary<Guid, string>> GetFloorLabelsAsync()
    {
        var hotelsResult = await _hotelService.GetAllAsync();
        var hotels = (hotelsResult.Data as List<Hotel> ?? new List<Hotel>()).ToDictionary(h => h.Id, h => h.Name);

        var floorsResult = await _floorService.GetAllAsync();
        var floors = floorsResult.Data as List<Floor> ?? new List<Floor>();

        return floors.ToDictionary(f => f.Id, f => $"Piso {f.Number} - {hotels.GetValueOrDefault(f.HotelId, "-")}");
    }

    private async Task LoadListsAsync()
    {
        var floorsResult = await _floorService.GetAllAsync();
        var activeFloors = (floorsResult.Data as List<Floor> ?? new List<Floor>()).Where(f => f.IsActive).ToList();
        var labels = await GetFloorLabelsAsync();

        ViewBag.Floors = new SelectList(
            activeFloors.Select(f => new { f.Id, Label = labels[f.Id] }), "Id", "Label");
        ViewBag.RoomTypes = new SelectList(await GetRoomTypesAsync(), "Id", "Name");
        ViewBag.Statuses = new SelectList(
            StatusNames.Select(s => new { Id = s.Key, Name = s.Value }), "Id", "Name");
    }
}