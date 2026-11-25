using HotelApp.BL.Dtos.Reservation;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class ReservationController : BaseApiController<SaveReservationDto, UpdateReservationDto>
{
    public ReservationController(IReservationService service) : base(service) { }
}