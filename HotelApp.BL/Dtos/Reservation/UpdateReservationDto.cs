using HotelApp.DAL.Models.Base;
namespace HotelApp.BL.Dtos.Reservation;
public class UpdateReservationDto : ReservationBase { public Guid Id { get; set; } }