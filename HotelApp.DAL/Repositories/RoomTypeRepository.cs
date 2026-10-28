using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.RoomType;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class RoomTypeRepository : BaseRepository<RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(RoomTypeAdd model)
    {
        var roomType = new RoomType
        {
            Name = model.Name,
            BaseRate = model.BaseRate
        };
        _context.RoomTypes.Add(roomType);
        return await SaveAsync("Tipo de habitación creado", roomType);
    }

    public async Task<OperationResult> UpdateAsync(RoomTypeUpdate model)
    {
        var roomType = await _context.RoomTypes.FindAsync(model.Id);
        if (roomType is null) return OperationResult.Fail("Tipo de habitación no encontrado");

        roomType.Name = model.Name;
        roomType.BaseRate = model.BaseRate;
        return await SaveAsync("Tipo de habitación actualizado", roomType);
    }
}