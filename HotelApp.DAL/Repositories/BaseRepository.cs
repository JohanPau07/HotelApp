using HotelApp.DAL.Data;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
        {
            return OperationResult.Fail(sql.Number switch
            {
                547 => "No se puede eliminar porque este registro está en uso por otros registros. Elimina o cambia primero los que dependen de él.",
                2601 or 2627 => "Ya existe un registro con esos datos. Revisa el nombre o número e inténtalo de nuevo.",
                _ => sql.Message
            });
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.InnerException?.Message ?? ex.Message);
        }
    }
}