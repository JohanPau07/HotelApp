using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Room;
using HotelApp.DAL.Models.Result;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.DAL.Repositories;

public class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    public RoomRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(RoomAdd model)
    {
        if (model.IsActive && await _context.Rooms.AnyAsync(r => r.FloorId == model.FloorId && r.Number == model.Number && r.IsActive))
            return OperationResult.Fail($"La habitación {model.Number} ya existe en este piso. Usa otro número.");

        var room = new Room
        {
            FloorId = model.FloorId,
            RoomTypeId = model.RoomTypeId,
            Number = model.Number,
            Capacity = model.Capacity,
            Rate = model.Rate,
            Status = model.Status,
            IsActive = model.IsActive,
            Description = model.Description
        };
        _context.Rooms.Add(room);
        return await SaveAsync("Habitación creada", room);
    }

    public async Task<OperationResult> UpdateAsync(RoomUpdate model)
    {
        var room = await _context.Rooms.FindAsync(model.Id);
        if (room is null) return OperationResult.Fail("Habitación no encontrada");

        if (model.IsActive && await _context.Rooms.AnyAsync(r => r.FloorId == model.FloorId && r.Number == model.Number && r.IsActive && r.Id != model.Id))
            return OperationResult.Fail($"La habitación {model.Number} ya existe en este piso. Usa otro número.");

        room.FloorId = model.FloorId;
        room.RoomTypeId = model.RoomTypeId;
        room.Number = model.Number;
        room.Capacity = model.Capacity;
        room.Rate = model.Rate;
        room.Status = model.Status;
        room.IsActive = model.IsActive;
        room.Description = model.Description;
        return await SaveAsync("Habitación actualizada", room);
    }
}