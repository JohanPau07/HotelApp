using HotelApp.BL.Dtos.Floor;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class FloorController : BaseApiController<SaveFloorDto, UpdateFloorDto>
{
    public FloorController(IFloorService service) : base(service) { }
}