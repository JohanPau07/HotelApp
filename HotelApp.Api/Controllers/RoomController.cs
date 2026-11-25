using HotelApp.BL.Dtos.Room;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class RoomController : BaseApiController<SaveRoomDto, UpdateRoomDto>
{
    public RoomController(IRoomService service) : base(service) { }
}