using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Hotel;

public sealed class HotelUpdate : HotelBase
{
    public Guid Id { get; set; }
}