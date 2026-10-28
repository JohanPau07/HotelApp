using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.RoomType;

public sealed class RoomTypeUpdate : RoomTypeBase
{
    public Guid Id { get; set; }
}