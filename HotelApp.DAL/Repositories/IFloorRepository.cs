using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Floor;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public interface IFloorRepository : IBaseRepository<Floor>
{
    Task<OperationResult> AddAsync(FloorAdd model);
    Task<OperationResult> UpdateAsync(FloorUpdate model);
}