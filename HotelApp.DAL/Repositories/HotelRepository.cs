using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Hotel;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
{
    public HotelRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(HotelAdd model)
    {
        var hotel = new Hotel
        {
            Name = model.Name,
            Address = model.Address,
            IsActive = model.IsActive
        };
        _context.Hotels.Add(hotel);
        return await SaveAsync("Hotel creado", hotel);
    }

    public async Task<OperationResult> UpdateAsync(HotelUpdate model)
    {
        var hotel = await _context.Hotels.FindAsync(model.Id);
        if (hotel is null) return OperationResult.Fail("Hotel no encontrado");

        hotel.Name = model.Name;
        hotel.Address = model.Address;
        hotel.IsActive = model.IsActive;
        return await SaveAsync("Hotel actualizado", hotel);
    }
}