using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Reservation;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Interfaces;

public interface IReservationRepository : IBaseRepository<Reservation>
{
    Task<OperationResult> AddAsync(ReservationAdd model);
    Task<OperationResult> UpdateAsync(ReservationUpdate model);
}