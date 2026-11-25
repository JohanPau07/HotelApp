using HotelApp.BL.Dtos.RoomType;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class RoomTypeController : BaseApiController<SaveRoomTypeDto, UpdateRoomTypeDto>
{
    public RoomTypeController(IRoomTypeService service) : base(service) { }
}