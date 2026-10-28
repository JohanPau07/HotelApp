using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Guest;

public sealed class GuestUpdate : GuestBase
{
    public Guid Id { get; set; }
}