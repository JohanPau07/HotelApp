using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Hotel;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

public class HotelController : Controller
{
    private readonly IHotelService _hotelService;

    public HotelController(IHotelService hotelService) => _hotelService = hotelService;

    public async Task<IActionResult> Index() => View(await BuildIndexModelAsync(null));

    public IActionResult Create() => View(new HotelFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelFormViewModel model)
    {
        var result = await _hotelService.SaveAsync(new SaveHotelDto
        {
            Name = model.Name,
            Address = model.Address,
            IsActive = model.IsActive,
            Stars = model.Stars,
            Description = model.Description,
            Services = model.Services
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
        var result = await _hotelService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Hotel hotel)
            return RedirectToAction(nameof(Index));

        return View(new HotelFormViewModel
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            IsActive = hotel.IsActive,
            Stars = hotel.Stars,
            Description = hotel.Description,
            Services = hotel.Services
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HotelFormViewModel model)
    {
        var result = await _hotelService.UpdateAsync(new UpdateHotelDto
        {
            Id = model.Id ?? Guid.Empty,
            Name = model.Name,
            Address = model.Address,
            IsActive = model.IsActive,
            Stars = model.Stars,
            Description = model.Description,
            Services = model.Services
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
        var result = await _hotelService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
            return View(nameof(Index), await BuildIndexModelAsync(result.Message));

        return RedirectToAction(nameof(Index));
    }

    private async Task<HotelIndexViewModel> BuildIndexModelAsync(string? error)
    {
        var model = new HotelIndexViewModel { Error = error };

        var result = await _hotelService.GetAllAsync();
        if (!result.Success)
        {
            model.Error ??= result.Message;
            return model;
        }

        var hotels = result.Data as List<Hotel> ?? new List<Hotel>();
        model.Items = hotels.OrderByDescending(h => h.CreatedAt).ToList();
        return model;
    }
}