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

    public async Task<IActionResult> Create(Guid? returnHotelId)
    {
        var model = new BlockFormViewModel
        {
            HotelId = returnHotelId ?? Guid.Empty,
            ReturnHotelId = returnHotelId,
            Hotels = await GetHotelOptionsAsync()
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlockFormViewModel model)
    {
        var result = await _blockService.SaveAsync(new SaveBlockDto
        {
            HotelId = model.HotelId,
            Name = model.Name,
            IsActive = model.IsActive
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { hotelId = model.ReturnHotelId });
    }

    public async Task<IActionResult> Edit(Guid id, Guid? returnHotelId)
    {
        var result = await _blockService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Block block)
            return RedirectToAction(nameof(Index), new { hotelId = returnHotelId });

        return View(new BlockFormViewModel
        {
            Id = block.Id,
            HotelId = block.HotelId,
            Name = block.Name,
            IsActive = block.IsActive,
            ReturnHotelId = returnHotelId,
            Hotels = await GetHotelOptionsAsync()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BlockFormViewModel model)
    {
        var result = await _blockService.UpdateAsync(new UpdateBlockDto
        {
            Id = model.Id ?? Guid.Empty,
            HotelId = model.HotelId,
            Name = model.Name,
            IsActive = model.IsActive
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { hotelId = model.ReturnHotelId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? hotelId)
    {
        var result = await _blockService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
        {
            var model = new BlockIndexViewModel { HotelId = hotelId, Error = result.Message };
            return View(nameof(Index), model);
        }

        return RedirectToAction(nameof(Index), new { hotelId });
    }

    private async Task<List<Hotel>> GetHotelsAsync()
    {
        var result = await _hotelService.GetAllAsync();
        return result.Data as List<Hotel> ?? new List<Hotel>();
    }

    private async Task<List<SelectListItem>> GetHotelOptionsAsync()
    {
        var hotels = await GetHotelsAsync();
        return hotels.Where(h => h.IsActive)
            .Select(h => new SelectListItem(h.Name, h.Id.ToString()))
            .ToList();
    }

    private async Task<IActionResult> ShowFormAsync(BlockFormViewModel model, string error)
    {
        model.Hotels = await GetHotelOptionsAsync();
        model.Error = error;
        return View(model);
    }
}