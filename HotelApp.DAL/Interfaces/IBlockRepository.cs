using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Objects.Block;
using HotelApp.DAL.Models.Result;

namespace HotelApp.DAL.Interfaces;

public interface IBlockRepository : IBaseRepository<Block>
{
    Task<OperationResult> AddAsync(BlockAdd model);
    Task<OperationResult> UpdateAsync(BlockUpdate model);
}