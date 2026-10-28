using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Floor;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class FloorRepository : BaseRepository<Floor>, IFloorRepository
{
    public FloorRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(FloorAdd model)
    {
        var floor = new Floor
        {
            HotelId = model.HotelId,
            Number = model.Number,
            IsActive = model.IsActive
        };
        _context.Floors.Add(floor);
        return await SaveAsync("Piso creado", floor);
    }

    public async Task<OperationResult> UpdateAsync(FloorUpdate model)
    {
        var floor = await _context.Floors.FindAsync(model.Id);
        if (floor is null) return OperationResult.Fail("Piso no encontrado");

        floor.HotelId = model.HotelId;
        floor.Number = model.Number;
        floor.IsActive = model.IsActive;
        return await SaveAsync("Piso actualizado", floor);
    }
}