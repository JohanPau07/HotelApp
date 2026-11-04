using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Guest;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Guest;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class GuestService : BaseService<Guest, SaveGuestDto, UpdateGuestDto>, IGuestService
{
    private readonly IGuestRepository _guestRepository;

    public GuestService(IGuestRepository repository) : base(repository)
    {
        _guestRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveGuestDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _guestRepository.AddAsync(new GuestAdd
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DocumentId = dto.DocumentId.Trim(),
            Email = dto.Email?.Trim(),
            Phone = dto.Phone?.Trim()
        });
    }

    public override Task<OperationResult> UpdateAsync(UpdateGuestDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _guestRepository.UpdateAsync(new GuestUpdate
        {
            Id = dto.Id,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DocumentId = dto.DocumentId.Trim(),
            Email = dto.Email?.Trim(),
            Phone = dto.Phone?.Trim()
        });
    }

    private static OperationResult? Validate(GuestBase dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return OperationResult.Fail("El nombre del huésped es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.LastName))
            return OperationResult.Fail("El apellido del huésped es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.DocumentId))
            return OperationResult.Fail("El documento de identidad es obligatorio.");
        if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains('@'))
            return OperationResult.Fail("El correo electrónico no es válido.");
        return null;
    }
}