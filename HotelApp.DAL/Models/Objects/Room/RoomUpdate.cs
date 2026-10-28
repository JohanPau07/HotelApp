using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Room;

public sealed class RoomUpdate : RoomBase
{
    public Guid Id { get; set; }
}