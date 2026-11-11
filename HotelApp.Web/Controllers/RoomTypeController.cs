using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.RoomType;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

public class RoomTypeController : Controller
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypeController(IRoomTypeService roomTypeService) => _roomTypeService = roomTypeService;

    public async Task<IActionResult> Index()
    {
        var result = await _roomTypeService.GetAllAsync();
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return View(new List<RoomType>());
        }
        return View(result.Data as List<RoomType> ?? new List<RoomType>());
    }

    public IActionResult Create() => View(new SaveRoomTypeDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveRoomTypeDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _roomTypeService.SaveAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        TempData["Ok"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _roomTypeService.GetByIdAsync(id);
        if (!result.Success || result.Data is not RoomType roomType)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(new UpdateRoomTypeDto
        {
            Id = roomType.Id,
            Name = roomType.Name,
            BaseRate = roomType.BaseRate
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRoomTypeDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _roomTypeService.UpdateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        TempData["Ok"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomTypeService.RemoveAsync(new RemoveDto { Id = id });
        TempData[result.Success ? "Ok" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}