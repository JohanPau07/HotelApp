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

    public override async Task<OperationResult> SaveAsync(SaveBlockDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var duplicateError = await ValidateDuplicateAsync(dto, null);
        if (duplicateError != null) return duplicateError;

        return await _blockRepository.AddAsync(new BlockAdd
        {
            HotelId = dto.HotelId,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        });
    }

    public override async Task<OperationResult> UpdateAsync(UpdateBlockDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var duplicateError = await ValidateDuplicateAsync(dto, dto.Id);
        if (duplicateError != null) return duplicateError;

        return await _blockRepository.UpdateAsync(new BlockUpdate
        {
            Id = dto.Id,
            HotelId = dto.HotelId,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        });
    }

    // RN: no puede haber dos bloques activos con el mismo nombre en el mismo hotel
    private async Task<OperationResult?> ValidateDuplicateAsync(BlockBase dto, Guid? excludeId)
    {
        if (!dto.IsActive) return null;

        var name = dto.Name.Trim();
        var exists = await _blockRepository.ExistsActiveAsync(dto.HotelId, name, excludeId);
        if (!exists) return null;

        return OperationResult.Fail(
            $"Ya existe un bloque llamado '{name}' activo en ese hotel. Usa otro nombre o desactiva el bloque existente.");
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