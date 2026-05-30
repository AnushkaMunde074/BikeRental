using BikeRental.API.DTOs;
using BikeRental.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessoriesController : ControllerBase
{
    private readonly IAccessoryService _accessoryService;

    public AccessoriesController(IAccessoryService accessoryService)
    {
        _accessoryService = accessoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccessoryDto>>> GetAccessories([FromQuery] string? bikeType = null)
    {
        var accessories = await _accessoryService.GetAccessoriesAsync(bikeType);
        return Ok(accessories);
    }

    [HttpPost("order")]
    public async Task<ActionResult<OrderResponse>> PlaceOrder([FromBody] List<AccessoryOrderItemRequest> items)
    {
        var result = await _accessoryService.ProcessOrderAsync(items);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
