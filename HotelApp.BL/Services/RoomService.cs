using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Room;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Room;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class RoomService : BaseService<Room, SaveRoomDto, UpdateRoomDto>, IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository repository) : base(repository)
    {
        _roomRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveRoomDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _roomRepository.AddAsync(new RoomAdd
        {
            FloorId = dto.FloorId,
            RoomTypeId = dto.RoomTypeId,
            Number = dto.Number.Trim(),
            Capacity = dto.Capacity,
            Rate = dto.Rate,
            Status = dto.Status,
            IsActive = dto.IsActive
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateRoomDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _roomRepository.UpdateAsync(new RoomUpdate
        {
            Id = dto.Id,
            FloorId = dto.FloorId,
            RoomTypeId = dto.RoomTypeId,
            Number = dto.Number.Trim(),
            Capacity = dto.Capacity,
            Rate = dto.Rate,
            Status = dto.Status,
            IsActive = dto.IsActive
        });
    }

    private static OperationResult? Validate(RoomBase dto)
    {
        if (dto.FloorId == Guid.Empty)
            return OperationResult.Fail("El piso de la habitación es obligatorio.");
        if (dto.RoomTypeId == Guid.Empty)
            return OperationResult.Fail("El tipo de habitación es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Number))
            return OperationResult.Fail("El número de habitación es obligatorio.");
        if (dto.Capacity <= 0)
            return OperationResult.Fail("La capacidad debe ser mayor que cero.");
        if (dto.Rate <= 0)
            return OperationResult.Fail("La tarifa debe ser mayor que cero.");
        if (dto.Status > 5)
            return OperationResult.Fail("El estado de la habitación no es válido.");
        return null;
    }
}