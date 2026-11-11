using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Floor;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Web.Controllers;

public class FloorController : Controller
{
    private readonly IFloorService _floorService;
    private readonly IHotelService _hotelService;
    private readonly IBlockService _blockService;

    public FloorController(IFloorService floorService, IHotelService hotelService, IBlockService blockService)
    {
        _floorService = floorService;
        _hotelService = hotelService;
        _blockService = blockService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _floorService.GetAllAsync();
        if (!result.Success)
            TempData["Error"] = result.Message;

        ViewBag.HotelNames = (await GetHotelsAsync()).ToDictionary(h => h.Id, h => h.Name);
        ViewBag.BlockNames = (await GetBlocksAsync()).ToDictionary(b => b.Id, b => b.Name);
        return View(result.Data as List<Floor> ?? new List<Floor>());
    }

    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        return View(new SaveFloorDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveFloorDto dto)
    {
        ModelState.Remove(nameof(dto.HotelId));
        await ValidateBlockAsync(dto);

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(dto);
        }

        var result = await _floorService.SaveAsync(dto);
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
        var result = await _floorService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Floor floor)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        await LoadListsAsync();
        return View(new UpdateFloorDto
        {
            Id = floor.Id,
            HotelId = floor.HotelId,
            BlockId = floor.BlockId,
            Number = floor.Number,
            IsActive = floor.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateFloorDto dto)
    {
        ModelState.Remove(nameof(dto.HotelId));
        await ValidateBlockAsync(dto);

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View(dto);
        }

        var result = await _floorService.UpdateAsync(dto);
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
        var result = await _floorService.RemoveAsync(new RemoveDto { Id = id });
        TempData[result.Success ? "Ok" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<Hotel>> GetHotelsAsync()
    {
        var result = await _hotelService.GetAllAsync();
        return result.Data as List<Hotel> ?? new List<Hotel>();
    }

    private async Task<List<Block>> GetBlocksAsync()
    {
        var result = await _blockService.GetAllAsync();
        return result.Data as List<Block> ?? new List<Block>();
    }

    private async Task LoadListsAsync()
    {
        var hotels = await GetHotelsAsync();
        var hotelNames = hotels.ToDictionary(h => h.Id, h => h.Name);
        var blocks = await GetBlocksAsync();

        ViewBag.Hotels = new SelectList(hotels.Where(h => h.IsActive), "Id", "Name");
        ViewBag.Blocks = new SelectList(
            blocks.Where(b => b.IsActive)
                  .Select(b => new { b.Id, Label = $"{b.Name} ({hotelNames.GetValueOrDefault(b.HotelId, "-")})" }),
            "Id", "Label");
    }

    private async Task ValidateBlockAsync(FloorBase dto)
    {
        if (dto.BlockId is null) return;

        var result = await _blockService.GetByIdAsync(dto.BlockId.Value);
        if (!result.Success || result.Data is not Block block)
        {
            ModelState.AddModelError(string.Empty, "El bloque seleccionado no existe.");
            return;
        }

        if (block.HotelId != dto.HotelId)
            ModelState.AddModelError(string.Empty, "El bloque no pertenece al hotel seleccionado.");
    }
}