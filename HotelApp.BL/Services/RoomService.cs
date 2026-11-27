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

    public override async Task<OperationResult> SaveAsync(SaveRoomDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var duplicateError = await ValidateDuplicateAsync(dto, null);
        if (duplicateError != null) return duplicateError;

        return await _roomRepository.AddAsync(new RoomAdd
        {
            FloorId = dto.FloorId,
            RoomTypeId = dto.RoomTypeId,
            Number = dto.Number.Trim(),
            Capacity = dto.Capacity,
            Rate = dto.Rate,
            Status = dto.Status,
            IsActive = dto.IsActive,
            Description = dto.Description?.Trim()
        });
    }

    public override async Task<OperationResult> UpdateAsync(UpdateRoomDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        var duplicateError = await ValidateDuplicateAsync(dto, dto.Id);
        if (duplicateError != null) return duplicateError;

        return await _roomRepository.UpdateAsync(new RoomUpdate
        {
            Id = dto.Id,
            FloorId = dto.FloorId,
            RoomTypeId = dto.RoomTypeId,
            Number = dto.Number.Trim(),
            Capacity = dto.Capacity,
            Rate = dto.Rate,
            Status = dto.Status,
            IsActive = dto.IsActive,
            Description = dto.Description?.Trim()
        });
    }

    // RN: no puede haber dos habitaciones activas con el mismo número en el mismo piso
    private async Task<OperationResult?> ValidateDuplicateAsync(RoomBase dto, Guid? excludeId)
    {
        if (!dto.IsActive) return null;

        var number = dto.Number.Trim();
        var exists = await _roomRepository.ExistsActiveAsync(dto.FloorId, number, excludeId);
        if (!exists) return null;

        return OperationResult.Fail(
            $"La habitación {number} ya existe en este piso. Usa otro número o desactiva la habitación existente.");
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
        if (dto.Description is { Length: > 500 })
            return OperationResult.Fail("La descripción de la habitación no puede superar los 500 caracteres.");
        return null;
    }
}