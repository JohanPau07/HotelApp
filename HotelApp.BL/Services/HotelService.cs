using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Hotel;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Hotel;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class HotelService : BaseService<Hotel, SaveHotelDto, UpdateHotelDto>, IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IHotelRepository repository) : base(repository)
    {
        _hotelRepository = repository;
    }

    public override async Task<OperationResult> SaveAsync(SaveHotelDto dto)
    {
        var error = Validate(dto);
        if (error is not null) return error;

        return await _hotelRepository.AddAsync(new HotelAdd
        {
            Name = dto.Name.Trim(),
            Address = dto.Address.Trim(),
            IsActive = dto.IsActive,
            Stars = dto.Stars,
            Description = dto.Description?.Trim(),
            Services = dto.Services?.Trim()
        });
    }

    public override async Task<OperationResult> UpdateAsync(UpdateHotelDto dto)
    {
        var error = Validate(dto);
        if (error is not null) return error;

        return await _hotelRepository.UpdateAsync(new HotelUpdate
        {
            Id = dto.Id,
            Name = dto.Name.Trim(),
            Address = dto.Address.Trim(),
            IsActive = dto.IsActive,
            Stars = dto.Stars,
            Description = dto.Description?.Trim(),
            Services = dto.Services?.Trim()
        });
    }

    private static OperationResult? Validate(HotelBase dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult.Fail("El nombre del hotel es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Address))
            return OperationResult.Fail("La dirección del hotel es obligatoria.");
        if (dto.Stars < 1 || dto.Stars > 5)
            return OperationResult.Fail("La calificación debe estar entre 1 y 5 estrellas.");
        if (dto.Description is { Length: > 1000 })
            return OperationResult.Fail("La descripción del hotel no puede superar los 1000 caracteres.");
        if (dto.Services is { Length: > 500 })
            return OperationResult.Fail("Los servicios del hotel no pueden superar los 500 caracteres.");
        return null;
    }
}