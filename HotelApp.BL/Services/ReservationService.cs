using HotelApp.BL.Base;
using HotelApp.BL.Dtos.Reservation;
using HotelApp.BL.Interfaces;
using HotelApp.DAL.Interfaces;
using HotelApp.DAL.Models;
using HotelApp.DAL.Models.Base;
using HotelApp.DAL.Models.Objects.Reservation;
using HotelApp.DAL.Models.Result;

namespace HotelApp.BL.Services;

public class ReservationService : BaseService<Reservation, SaveReservationDto, UpdateReservationDto>, IReservationService
{
    private const byte Completed = 3;
    private const byte Cancelled = 4;

    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository repository) : base(repository)
    {
        _reservationRepository = repository;
    }

    public override Task<OperationResult> SaveAsync(SaveReservationDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Task.FromResult(error);

        return _reservationRepository.AddAsync(new ReservationAdd
        {
            RoomId = dto.RoomId,
            GuestId = dto.GuestId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            GuestCount = dto.GuestCount,
            TotalPrice = dto.TotalPrice,
            Status = dto.Status
        });
    }

    public override async Task<OperationResult> UpdateAsync(UpdateReservationDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        // RN-020: una reserva Completed o Cancelled no puede modificarse
        var current = await _reservationRepository.GetByIdAsync(dto.Id);
        if (!current.Success) return current;

        if (current.Data is Reservation existing &&
            (existing.Status == Completed || existing.Status == Cancelled))
            return OperationResult.Fail("Una reserva completada o cancelada no puede modificarse.");

        return await _reservationRepository.UpdateAsync(new ReservationUpdate
        {
            Id = dto.Id,
            RoomId = dto.RoomId,
            GuestId = dto.GuestId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            GuestCount = dto.GuestCount,
            TotalPrice = dto.TotalPrice,
            Status = dto.Status
        });
    }

    private static OperationResult? Validate(ReservationBase dto)
    {
        if (dto.RoomId == Guid.Empty)
            return OperationResult.Fail("La habitación es obligatoria.");
        if (dto.GuestId == Guid.Empty)
            return OperationResult.Fail("El huésped titular es obligatorio.");
        if (dto.CheckOut <= dto.CheckIn)
            return OperationResult.Fail("La fecha de salida debe ser posterior a la de entrada.");
        if (dto.GuestCount <= 0)
            return OperationResult.Fail("La cantidad de huéspedes debe ser mayor que cero.");
        if (dto.TotalPrice < 0)
            return OperationResult.Fail("El precio total no puede ser negativo.");
        if (dto.Status > 4)
            return OperationResult.Fail("El estado de la reserva no es válido.");
        return null;
    }
}