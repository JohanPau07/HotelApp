using HotelApp.BL.Dtos.Guest;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class GuestController : BaseApiController<SaveGuestDto, UpdateGuestDto>
{
    public GuestController(IGuestService service) : base(service) { }
}