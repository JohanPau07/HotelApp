using HotelApp.DAL.Data;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Reservation;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(HotelDbContext context) : base(context) { }

    public async Task<OperationResult> AddAsync(ReservationAdd model)
    {
        // CreatedAt y UpdatedAt los pone la base de datos por defecto
        var reservation = new Reservation
        {
            RoomId = model.RoomId,
            GuestId = model.GuestId,
            CheckIn = model.CheckIn,
            CheckOut = model.CheckOut,
            GuestCount = model.GuestCount,
            TotalPrice = model.TotalPrice,
            Status = model.Status
        };
        _context.Reservations.Add(reservation);
        return await SaveAsync("Reserva creada", reservation);
    }

    public async Task<OperationResult> UpdateAsync(ReservationUpdate model)
    {
        var reservation = await _context.Reservations.FindAsync(model.Id);
        if (reservation is null) return OperationResult.Fail("Reserva no encontrada");

        reservation.RoomId = model.RoomId;
        reservation.GuestId = model.GuestId;
        reservation.CheckIn = model.CheckIn;
        reservation.CheckOut = model.CheckOut;
        reservation.GuestCount = model.GuestCount;
        reservation.TotalPrice = model.TotalPrice;
        reservation.Status = model.Status;
        reservation.UpdatedAt = DateTime.Now;
        return await SaveAsync("Reserva actualizada", reservation);
    }
}