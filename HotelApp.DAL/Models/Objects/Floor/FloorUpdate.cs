using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Floor;

public sealed class FloorUpdate : FloorBase
{
    public Guid Id { get; set; }
}