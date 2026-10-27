using HotelApp.DAL.Data;
using HotelApp.DAL.Models.Result;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.DAL.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly HotelDbContext _context;

    protected BaseRepository(HotelDbContext context) => _context = context;

    public virtual async Task<OperationResult> GetAllAsync() =>
        OperationResult.Ok("Registros obtenidos",
            await _context.Set<TEntity>().AsNoTracking().ToListAsync());

    public virtual async Task<OperationResult> GetByIdAsync(Guid id)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);
        return entity is null
            ? OperationResult.Fail("Registro no encontrado")
            : OperationResult.Ok("Registro obtenido", entity);
    }

    public virtual async Task<OperationResult> RemoveAsync(Guid id)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);
        if (entity is null) return OperationResult.Fail("Registro no encontrado");

        _context.Set<TEntity>().Remove(entity);
        return await SaveAsync("Registro eliminado");
    }


    protected async Task<OperationResult> SaveAsync(string okMessage, object? data = null)
    {
        try
        {
            await _context.SaveChangesAsync();
            return OperationResult.Ok(okMessage, data);
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.InnerException?.Message ?? ex.Message);
        }
    }
}