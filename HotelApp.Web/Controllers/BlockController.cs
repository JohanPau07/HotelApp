using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Block;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Controllers;

public class BlockController : Controller
{
    private readonly IBlockService _blockService;
    private readonly IHotelService _hotelService;

    public BlockController(IBlockService blockService, IHotelService hotelService)
    {
        _blockService = blockService;
        _hotelService = hotelService;
    }

    public async Task<IActionResult> Index(Guid? hotelId)
    {
        var model = new BlockIndexViewModel { HotelId = hotelId };

        var result = await _blockService.GetAllAsync();
        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        var hotels = await GetHotelsAsync();
        model.HotelNames = hotels.ToDictionary(h => h.Id, h => h.Name);
        if (hotelId.HasValue)
            model.HotelName = model.HotelNames.GetValueOrDefault(hotelId.Value);

        var blocks = result.Data as List<Block> ?? new List<Block>();
        if (hotelId.HasValue)
            blocks = blocks.Where(b => b.HotelId == hotelId.Value).ToList();

        model.Items = blocks.OrderByDescending(b => b.CreatedAt).ToList();
        return View(model);
    }
    public async Task<IActionResult> Create()
    {
        await LoadHotelsAsync();
        return View(new SaveBlockDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveBlockDto dto)
    {
        ModelState.Remove(nameof(dto.HotelId));

        if (!ModelState.IsValid)
        {
            await LoadHotelsAsync();
            return View(dto);
        }

        var result = await _blockService.SaveAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadHotelsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index), new { hotelId = dto.HotelId });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _blockService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Block block)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        await LoadHotelsAsync();
        return View(new UpdateBlockDto
        {
            Id = block.Id,
            HotelId = block.HotelId,
            Name = block.Name,
            IsActive = block.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBlockDto dto)
    {
        ModelState.Remove(nameof(dto.HotelId));

        if (!ModelState.IsValid)
        {
            await LoadHotelsAsync();
            return View(dto);
        }

        var result = await _blockService.UpdateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadHotelsAsync();
            return View(dto);
        }

        return RedirectToAction(nameof(Index), new { hotelId = dto.HotelId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _blockService.RemoveAsync(new RemoveDto { Id = id });
        TempData[result.Success ? "Ok" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<Hotel>> GetHotelsAsync()
    {
        var result = await _hotelService.GetAllAsync();
        return result.Data as List<Hotel> ?? new List<Hotel>();
    }

    private async Task LoadHotelsAsync()
    {
        var hotels = await GetHotelsAsync();
        ViewBag.Hotels = new SelectList(hotels.Where(h => h.IsActive), "Id", "Name");
    }
}