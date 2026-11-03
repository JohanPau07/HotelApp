using HotelApp.BL.Dtos;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Interfaces;

public interface IBaseService<TSaveDto, TUpdateDto>
{
    Task<OperationResult> GetAllAsync();
    Task<OperationResult> GetByIdAsync(Guid id);
    Task<OperationResult> SaveAsync(TSaveDto dto);
    Task<OperationResult> UpdateAsync(TUpdateDto dto);
    Task<OperationResult> RemoveAsync(RemoveDto dto);
}