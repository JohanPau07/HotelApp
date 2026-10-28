using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<OperationResult> GetAllAsync();
    Task<OperationResult> GetByIdAsync(Guid id);
    Task<OperationResult> RemoveAsync(Guid id);
}