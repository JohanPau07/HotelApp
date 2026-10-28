using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Room;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Repositories;

public interface IRoomRepository : IBaseRepository<Room>
{
    Task<OperationResult> AddAsync(RoomAdd model);
    Task<OperationResult> UpdateAsync(RoomUpdate model);
}