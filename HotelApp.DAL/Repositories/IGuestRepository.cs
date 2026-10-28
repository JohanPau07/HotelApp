using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Guest;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public interface IGuestRepository : IBaseRepository<Guest>
{
    Task<OperationResult> AddAsync(GuestAdd model);
    Task<OperationResult> UpdateAsync(GuestUpdate model);
}