using HotelApp.BL.Base;
using HotelApp.BL.Dtos.RoomType;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.RoomType;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class RoomTypeService : BaseService<RoomType, SaveRoomTypeDto, UpdateRoomTypeDto>, IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository repository) : base(repository)
    {
        _roomTypeRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveRoomTypeDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _roomTypeRepository.AddAsync(new RoomTypeAdd
        {
            Name = dto.Name.Trim(),
            BaseRate = dto.BaseRate
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateRoomTypeDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _roomTypeRepository.UpdateAsync(new RoomTypeUpdate
        {
            Id = dto.Id,
            Name = dto.Name.Trim(),
            BaseRate = dto.BaseRate
        });
    }

    private static OperationResult? Validate(RoomTypeBase dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult.Fail("El nombre del tipo de habitación es obligatorio.");
        if (dto.BaseRate <= 0)
            return OperationResult.Fail("La tarifa base debe ser mayor que cero.");
        return null;
    }
}