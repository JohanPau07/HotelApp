using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.RoomType;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

public class RoomTypeController : Controller
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypeController(IRoomTypeService roomTypeService) => _roomTypeService = roomTypeService;

    public async Task<IActionResult> Index() => View(await BuildIndexModelAsync(null));

    public IActionResult Create() => View(new RoomTypeFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomTypeFormViewModel model)
    {
        var result = await _roomTypeService.SaveAsync(new SaveRoomTypeDto
        {
            Name = model.Name,
            BaseRate = model.BaseRate,
            Capacity = model.Capacity
        });

        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _roomTypeService.GetByIdAsync(id);
        if (!result.Success || result.Data is not RoomType roomType)
            return RedirectToAction(nameof(Index));

        return View(new RoomTypeFormViewModel
        {
            Id = roomType.Id,
            Name = roomType.Name,
            BaseRate = roomType.BaseRate,
            Capacity = roomType.Capacity
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoomTypeFormViewModel model)
    {
        var result = await _roomTypeService.UpdateAsync(new UpdateRoomTypeDto
        {
            Id = model.Id ?? Guid.Empty,
            Name = model.Name,
            BaseRate = model.BaseRate,
            Capacity = model.Capacity
        });

        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomTypeService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
            return View(nameof(Index), await BuildIndexModelAsync(result.Message));

        return RedirectToAction(nameof(Index));
    }

    private async Task<RoomTypeIndexViewModel> BuildIndexModelAsync(string? error)
    {
        var model = new RoomTypeIndexViewModel { Error = error };

        var result = await _roomTypeService.GetAllAsync();
        if (!result.Success)
        {
            model.Error ??= result.Message;
            return model;
        }

        var roomTypes = result.Data as List<RoomType> ?? new List<RoomType>();
        model.Items = roomTypes.OrderByDescending(t => t.CreatedAt).ToList();
        return model;
    }
}