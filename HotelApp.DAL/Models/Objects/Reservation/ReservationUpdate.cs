using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Reservation;

public sealed class ReservationUpdate : ReservationBase
{
    public Guid Id { get; set; }
}