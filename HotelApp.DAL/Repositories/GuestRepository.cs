using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Guest;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class GuestRepository : BaseRepository<Guest>, IGuestRepository
{
    public GuestRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(GuestAdd model)
    {
        var guest = new Guest
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DocumentId = model.DocumentId,
            Email = model.Email,
            Phone = model.Phone
        };
        _context.Guests.Add(guest);
        return await SaveAsync("Huésped creado", guest);
    }

    public async Task<OperationResult> UpdateAsync(GuestUpdate model)
    {
        var guest = await _context.Guests.FindAsync(model.Id);
        if (guest is null) return OperationResult.Fail("Huésped no encontrado");

        guest.FirstName = model.FirstName;
        guest.LastName = model.LastName;
        guest.DocumentId = model.DocumentId;
        guest.Email = model.Email;
        guest.Phone = model.Phone;
        return await SaveAsync("Huésped actualizado", guest);
    }
}