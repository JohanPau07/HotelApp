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
    private readonly IRoomRepository _roomRepository;

    public ReservationService(IReservationRepository repository, IRoomRepository roomRepository) : base(repository)
    {
        _reservationRepository = repository;
        _roomRepository = roomRepository;
    }

    public override async Task<OperationResult> SaveAsync(SaveReservationDto dto)
    {
        var error = Validate(dto);
        if (error != null) return error;

        if (dto.CheckIn < DateOnly.FromDateTime(DateTime.Today))
            return OperationResult.Fail("No se pueden hacer reservas en fechas pasadas. Elige la fecha de hoy o una fecha futura.");

        var capacityError = await ValidateCapacityAsync(dto);
        if (capacityError != null) return capacityError;

        var availabilityError = await ValidateAvailabilityAsync(dto, null);
        if (availabilityError != null) return availabilityError;

        return await _reservationRepository.AddAsync(new ReservationAdd
        {
            RoomId = dto.RoomId,
            GuestId = dto.GuestId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            GuestCount = dto.GuestCount,
            TotalPrice = await CalculateTotalAsync(dto),
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

        var existing = current.Data as Reservation;
        if (existing is not null && (existing.Status == Completed || existing.Status == Cancelled))
            return OperationResult.Fail("Una reserva completada o cancelada no puede modificarse.");

        var capacityError = await ValidateCapacityAsync(dto);
        if (capacityError != null) return capacityError;

        
        var isCancelling = dto.Status == Cancelled;
        if (!isCancelling)
        {
            var availabilityError = await ValidateAvailabilityAsync(dto, dto.Id);
            if (availabilityError != null) return availabilityError;
        }

        var totalPrice = isCancelling && existing is not null
            ? existing.TotalPrice
            : await CalculateTotalAsync(dto);

        return await _reservationRepository.UpdateAsync(new ReservationUpdate
        {
            Id = dto.Id,
            RoomId = dto.RoomId,
            GuestId = dto.GuestId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            GuestCount = dto.GuestCount,
            TotalPrice = totalPrice,
            Status = dto.Status
        });
    }

    private async Task<OperationResult?> ValidateCapacityAsync(ReservationBase dto)
    {
        var roomResult = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (!roomResult.Success || roomResult.Data is not Room room)
            return OperationResult.Fail("La habitación seleccionada no existe.");

        if (dto.GuestCount > room.Capacity)
            return OperationResult.Fail(
                $"La habitación {room.Number} permite máximo {room.Capacity} personas y se indicaron {dto.GuestCount}. " +
                "Elige una habitación con mayor capacidad o divide el grupo en más de una habitación.");

        return null;
    }

    private async Task<OperationResult?> ValidateAvailabilityAsync(ReservationBase dto, Guid? excludeId)
    {
        var overlaps = await _reservationRepository.HasOverlapAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, excludeId);
        if (overlaps)
            return OperationResult.Fail(
                "La habitación ya está reservada en esas fechas. Elige otras fechas u otra habitación.");

        return null;
    }

    // noches x tarifa de la habitación
    private async Task<decimal> CalculateTotalAsync(ReservationBase dto)
    {
        var roomResult = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (roomResult.Data is not Room room) return 0;

        var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber;
        return nights * room.Rate;
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
        if (dto.Status > 4)
            return OperationResult.Fail("El estado de la reserva no es válido.");
        return null;
    }
}