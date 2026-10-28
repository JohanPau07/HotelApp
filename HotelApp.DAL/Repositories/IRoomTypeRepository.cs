using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.RoomType;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public interface IRoomTypeRepository : IBaseRepository<RoomType>
{
    Task<OperationResult> AddAsync(RoomTypeAdd model);
    Task<OperationResult> UpdateAsync(RoomTypeUpdate model);
}