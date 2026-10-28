using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Hotel;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Interfaces;

public interface IHotelRepository : IBaseRepository<Hotel>
{
    Task<OperationResult> AddAsync(HotelAdd model);
    Task<OperationResult> UpdateAsync(HotelUpdate model);
}