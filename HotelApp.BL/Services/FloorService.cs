using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Floor;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Floor;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class FloorService : BaseService<Floor, SaveFloorDto, UpdateFloorDto>, IFloorService
{
    private readonly IFloorRepository _floorRepository;
    private readonly IBlockRepository _blockRepository;

    public FloorService(IFloorRepository repository, IBlockRepository blockRepository) : base(repository)
    {
        _floorRepository = repository;
        _blockRepository = blockRepository;
    }

    public override async Task<OperationResult> SaveAsync(SaveFloorDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var blockError = await ValidateBlockAsync(dto);
        if (blockError != null) return blockError;

        return await _floorRepository.AddAsync(new FloorAdd
        {
            HotelId = dto.HotelId,
            BlockId = dto.BlockId,
            Number = dto.Number,
            IsActive = dto.IsActive
        });
    }

    public override async Task<OperationResult> UpdateAsync(UpdateFloorDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var blockError = await ValidateBlockAsync(dto);
        if (blockError != null) return blockError;

        return await _floorRepository.UpdateAsync(new FloorUpdate
        {
            Id = dto.Id,
            HotelId = dto.HotelId,
            BlockId = dto.BlockId,
            Number = dto.Number,
            IsActive = dto.IsActive
        });
    }

    // RN: el bloque del piso debe existir y pertenecer al mismo hotel del piso
    private async Task<OperationResult?> ValidateBlockAsync(FloorBase dto)
    {
        if (dto.BlockId is null) return null;

        var blockResult = await _blockRepository.GetByIdAsync(dto.BlockId.Value);
        if (!blockResult.Success || blockResult.Data is not Block block)
            return OperationResult.Fail("El bloque seleccionado no existe.");

        if (block.HotelId != dto.HotelId)
            return OperationResult.Fail("El bloque no pertenece al hotel seleccionado.");

        return null;
    }

    private static OperationResult? Validate(FloorBase dto)
    {
        if (dto.HotelId == Guid.Empty)
            return OperationResult.Fail("El hotel del piso es obligatorio.");

        if (dto.Number <= 0)
            return OperationResult.Fail("El número de piso debe ser mayor que cero.");

        return null;
    }
}