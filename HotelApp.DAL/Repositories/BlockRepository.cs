using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Block;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class BlockRepository : BaseRepository<Block>, IBlockRepository
{
    public BlockRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(BlockAdd model)
    {
        var block = new Block
        {
            HotelId = model.HotelId,
            Name = model.Name,
            IsActive = model.IsActive
        };
        _context.Blocks.Add(block);
        return await SaveAsync("Bloque creado", block);
    }

    public async Task<OperationResult> UpdateAsync(BlockUpdate model)
    {
        var block = await _context.Blocks.FindAsync(model.Id);
        if (block is null) return OperationResult.Fail("Bloque no encontrado");

        block.HotelId = model.HotelId;
        block.Name = model.Name;
        block.IsActive = model.IsActive;
        return await SaveAsync("Bloque actualizado", block);
    }
}