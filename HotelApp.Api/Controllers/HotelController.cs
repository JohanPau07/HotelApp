using HotelApp.BL.Dtos.Hotel;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class HotelController : BaseApiController<SaveHotelDto, UpdateHotelDto>
{
    public HotelController(IHotelService service) : base(service) { }
}