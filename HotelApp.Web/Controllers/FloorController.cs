using HotelApp.BL.Dtos;
using HotelApp.BL.Dtos.Floor;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.Web.Models;
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

    public async Task<IActionResult> Index(Guid? blockId)
    {
        var model = new FloorIndexViewModel { BlockId = blockId };

        var result = await _floorService.GetAllAsync();
        if (!result.Success)
        {
            model.Error = result.Message;
            return View(model);
        }

        model.HotelNames = (await GetHotelsAsync()).ToDictionary(h => h.Id, h => h.Name);
        model.BlockNames = (await GetBlocksAsync()).ToDictionary(b => b.Id, b => b.Name);
        if (blockId.HasValue)
            model.BlockName = model.BlockNames.GetValueOrDefault(blockId.Value);

        var floors = result.Data as List<Floor> ?? new List<Floor>();
        if (blockId.HasValue)
            floors = floors.Where(f => f.BlockId == blockId.Value).ToList();

        model.Items = floors.OrderByDescending(f => f.CreatedAt).ToList();
        return View(model);
    }

    public async Task<IActionResult> Create(Guid? returnBlockId)
    {
        var model = new FloorFormViewModel { ReturnBlockId = returnBlockId, BlockId = returnBlockId };

        // Si vienes de un bloque, el bloque y su hotel ya vienen seleccionados
        if (returnBlockId.HasValue)
        {
            var block = (await GetBlocksAsync()).FirstOrDefault(b => b.Id == returnBlockId.Value);
            if (block is not null)
                model.HotelId = block.HotelId;
        }

        await LoadOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FloorFormViewModel model)
    {

        var result = await _floorService.SaveAsync(new SaveFloorDto
        {
            HotelId = model.HotelId,
            BlockId = model.BlockId,
            Number = model.Number,
            IsActive = model.IsActive
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { blockId = model.ReturnBlockId });
    }

    public async Task<IActionResult> Edit(Guid id, Guid? returnBlockId)
    {
        var result = await _floorService.GetByIdAsync(id);
        if (!result.Success || result.Data is not Floor floor)
            return RedirectToAction(nameof(Index), new { blockId = returnBlockId });

        var model = new FloorFormViewModel
        {
            Id = floor.Id,
            HotelId = floor.HotelId,
            BlockId = floor.BlockId,
            Number = floor.Number,
            IsActive = floor.IsActive,
            ReturnBlockId = returnBlockId
        };

        await LoadOptionsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FloorFormViewModel model)
    {

        var result = await _floorService.UpdateAsync(new UpdateFloorDto
        {
            Id = model.Id ?? Guid.Empty,
            HotelId = model.HotelId,
            BlockId = model.BlockId,
            Number = model.Number,
            IsActive = model.IsActive
        });

        if (!result.Success)
            return await ShowFormAsync(model, result.Message);

        return RedirectToAction(nameof(Index), new { blockId = model.ReturnBlockId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? blockId)
    {
        var result = await _floorService.RemoveAsync(new RemoveDto { Id = id });
        if (!result.Success)
        {
            var model = new FloorIndexViewModel { BlockId = blockId, Error = result.Message };
            return View(nameof(Index), model);
        }

        return RedirectToAction(nameof(Index), new { blockId });
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

    private async Task LoadOptionsAsync(FloorFormViewModel model)
    {
        var hotels = await GetHotelsAsync();
        var hotelNames = hotels.ToDictionary(h => h.Id, h => h.Name);
        var blocks = await GetBlocksAsync();

        model.Hotels = hotels.Where(h => h.IsActive)
            .Select(h => new SelectListItem(h.Name, h.Id.ToString()))
            .ToList();

        model.Blocks = blocks.Where(b => b.IsActive)
            .Select(b => new SelectListItem(
                $"{b.Name} ({hotelNames.GetValueOrDefault(b.HotelId, "-")})", b.Id.ToString()))
            .ToList();
    }



    private async Task<IActionResult> ShowFormAsync(FloorFormViewModel model, string error)
    {
        await LoadOptionsAsync(model);
        model.Error = error;
        return View(model);
    }
}