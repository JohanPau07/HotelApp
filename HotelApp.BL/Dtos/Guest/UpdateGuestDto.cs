using HotelApp.DAL.Models.Base;
namespace HotelApp.BL.Dtos.Guest;
public class UpdateGuestDto : GuestBase { public Guid Id { get; set; } }