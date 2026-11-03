using HotelApp.DAL.Models.Base;

namespace HotelApp.BL.Dtos.Hotel;

public sealed class UpdateHotelDto : HotelBase
{
    public Guid Id { get; set; }
}