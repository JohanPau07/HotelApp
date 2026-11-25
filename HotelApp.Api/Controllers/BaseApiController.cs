using HotelApp.BL.Dtos;
using HotelApp.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController<TSave, TUpdate> : ControllerBase
{
    private readonly IBaseService<TSave, TUpdate> _service;

    protected BaseApiController(IBaseService<TSave, TUpdate> service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetAllAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TSave dto)
    {
        var result = await _service.SaveAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] TUpdate dto)
    {
        var result = await _service.UpdateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.RemoveAsync(new RemoveDto { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}