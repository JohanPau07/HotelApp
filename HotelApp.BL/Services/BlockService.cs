using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Block;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Block;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class BlockService : BaseService<Block, SaveBlockDto, UpdateBlockDto>, IBlockService
{
    private readonly IBlockRepository _blockRepository;

    public BlockService(IBlockRepository repository) : base(repository)
    {
        _blockRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveBlockDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _blockRepository.AddAsync(new BlockAdd
        {
            HotelId = dto.HotelId,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateBlockDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _blockRepository.UpdateAsync(new BlockUpdate
        {
            Id = dto.Id,
            HotelId = dto.HotelId,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        });
    }

    private static OperationResult? Validate(BlockBase dto)
    {
        if (dto.HotelId == Guid.Empty)
            return OperationResult.Fail("El hotel del bloque es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult.Fail("El nombre del bloque es obligatorio.");

        return null;
    }
}