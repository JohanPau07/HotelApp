using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Hotel;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

public class HotelController : Controller
{
    private readonly IHotelService _hotelService;

    public HotelController(IHotelService hotelService) => _hotelService = hotelService;

    public async Task<IActionResult> Index()
    {
        var result = await _hotelService.GetAllAsync();
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return View(new List<Hotel>());
        }
        return View(result.Data as List<Hotel> ?? new List<Hotel>());
    }

    public IActionResult Create() => View(new SaveHotelDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveHotelDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _hotelService.SaveAsync(dto);
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
        var result = await _hotelService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Hotel hotel)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(new UpdateHotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            IsActive = hotel.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateHotelDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _hotelService.UpdateAsync(dto);
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
        var result = await _hotelService.RemoveAsync(new RemoveDto { Id = id });
        TempData[result.Success ? "Ok" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}