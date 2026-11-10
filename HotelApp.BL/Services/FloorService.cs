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

    public FloorService(IFloorRepository repository) : base(repository)
    {
        _floorRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveFloorDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _floorRepository.AddAsync(new FloorAdd
        {
            HotelId = dto.HotelId,
            BlockId = dto.BlockId,
            Number = dto.Number,
            IsActive = dto.IsActive
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateFloorDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _floorRepository.UpdateAsync(new FloorUpdate
        {
            Id = dto.Id,
            HotelId = dto.HotelId,
            BlockId = dto.BlockId,
            Number = dto.Number,
            IsActive = dto.IsActive
        });
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