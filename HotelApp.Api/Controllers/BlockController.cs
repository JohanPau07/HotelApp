using HotelApp.BL.Dtos.Block;
using HotelApp.BL.Interfaces;

namespace HotelApp.Api.Controllers;

public class BlockController : BaseApiController<SaveBlockDto, UpdateBlockDto>
{
    public BlockController(IBlockService service) : base(service) { }
}