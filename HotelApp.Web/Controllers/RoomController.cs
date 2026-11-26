using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Room;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
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

    public async Task<IActionResult> Index(Guid? floorId)
    {
        var model = new RoomIndexViewModel { FloorId = floorId };

        var result = await _roomService.GetAllAsync();
        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        model.FloorLabels = await GetFloorLabelsAsync();
        model.RoomTypeNames = (await GetRoomTypesAsync()).ToDictionary(t => t.Id, t => t.Name);
        model.StatusNames = StatusNames;
        if (floorId.HasValue)
            model.FloorLabel = model.FloorLabels.GetValueOrDefault(floorId.Value);

        var rooms = result.Data as List<Room> ?? new List<Room>();
        if (floorId.HasValue)
            rooms = rooms.Where(r => r.FloorId == floorId.Value).ToList();

        model.Items = rooms.OrderByDescending(r => r.CreatedAt).ToList();
        return View(model);
    }

    public async Task<IActionResult> Create(Guid? returnFloorId)
    {
        var model = new RoomFormViewModel
        {
            FloorId = returnFloorId ?? Guid.Empty,
            ReturnFloorId = returnFloorId
        };

        await LoadOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomFormViewModel model)
    {
        var result = await _roomService.SaveAsync(new SaveRoomDto
        {
            FloorId = model.FloorId,
            RoomTypeId = model.RoomTypeId,
            Number = model.Number,
            Capacity = model.Capacity,
            Rate = model.Rate,
            Status = model.Status,
            IsActive = model.IsActive,
            Description = model.Description
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { floorId = model.ReturnFloorId });
    }

    public async Task<IActionResult> Edit(Guid id, Guid? returnFloorId)
    {
        var result = await _roomService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Room room)
            return RedirectToAction(nameof(Index), new { floorId = returnFloorId });

        var model = new RoomFormViewModel
        {
            Id = room.Id,
            FloorId = room.FloorId,
            RoomTypeId = room.RoomTypeId,
            Number = room.Number,
            Capacity = room.Capacity,
            Rate = room.Rate,
            Status = room.Status,
            IsActive = room.IsActive,
            Description = room.Description,
            ReturnFloorId = returnFloorId
        };

        await LoadOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoomFormViewModel model)
    {
        var result = await _roomService.UpdateAsync(new UpdateRoomDto
        {
            Id = model.Id ?? Guid.Empty,
            FloorId = model.FloorId,
            RoomTypeId = model.RoomTypeId,
            Number = model.Number,
            Capacity = model.Capacity,
            Rate = model.Rate,
            Status = model.Status,
            IsActive = model.IsActive,
            Description = model.Description
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { floorId = model.ReturnFloorId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? floorId)
    {
        var result = await _roomService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
        {
            var model = new RoomIndexViewModel { FloorId = floorId, Error = result.Message };
            return View(nameof(Index), model);
        }

        return RedirectToAction(nameof(Index), new { floorId });
    }

    private async Task<List<RoomType>> GetRoomTypesAsync()
    {
        var result = await _roomTypeService.GetAllAsync();
        return result.Data as List<RoomType> ?? new List<RoomType>();
    }

    private async Task<List<Floor>> GetFloorsAsync()
    {
        var result = await _floorService.GetAllAsync();
        return result.Data as List<Floor> ?? new List<Floor>();
    }

    private async Task<Dictionary<Guid, string>> GetFloorLabelsAsync()
    {
        var hotelsResult = await _hotelService.GetAllAsync();
        var hotels = (hotelsResult.Data as List<Hotel> ?? new List<Hotel>()).ToDictionary(h => h.Id, h => h.Name);

        var floors = await GetFloorsAsync();
        return floors.ToDictionary(f => f.Id, f => $"Piso {f.Number} - {hotels.GetValueOrDefault(f.HotelId, "-")}");
    }

    private async Task LoadOptionsAsync(RoomFormViewModel model)
    {
        var labels = await GetFloorLabelsAsync();

        model.Floors = (await GetFloorsAsync())
            .Where(f => f.IsActive)
            .Select(f => new SelectListItem(labels[f.Id], f.Id.ToString()))
            .ToList();

        model.RoomTypes = (await GetRoomTypesAsync())
            .Select(t => new RoomTypeOptionViewModel
            {
                Id = t.Id,
                Name = t.Name,
                BaseRate = t.BaseRate,
                Capacity = t.Capacity
            })
            .ToList();

        model.Statuses = StatusNames
            .Select(s => new SelectListItem(s.Value, s.Key.ToString()))
            .ToList();
    }

    private async Task<IActionResult> ShowFormAsync(RoomFormViewModel model, string error)
    {
        await LoadOptionsAsync(model);
        model.Error = error;
        return View(model);
    }
}