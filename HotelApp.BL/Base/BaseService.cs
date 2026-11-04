using HotelApp.BL.Dtos;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Base;

public abstract class BaseService<TEntity, TSaveDto, TUpdateDto> : IBaseService<TSaveDto, TUpdateDto>
    where TEntity : class
{
    protected readonly IBaseRepository<TEntity> _repository;

    protected BaseService(IBaseRepository<TEntity> repository) => _repository = repository;

    public virtual Task<OperationResult> GetAllAsync() => _repository.GetAllAsync();
    public virtual Task<OperationResult> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public virtual Task<OperationResult> RemoveAsync(RemoveDto dto) => _repository.RemoveAsync(dto.Id);
    public abstract Task<OperationResult> SaveAsync(TSaveDto dto);
    public abstract Task<OperationResult> UpdateAsync(TUpdateDto dto);
}
