using BikeRental.API.DTOs;
using BikeRental.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikesController : ControllerBase
{
    private readonly IBikeService _bikeService;

    public BikesController(IBikeService bikeService)
    {
        _bikeService = bikeService;
    }

    [HttpGet("beach")]
    public async Task<ActionResult<List<BeachCruiserDto>>> GetBeachCruisers()
    {
        var bikes = await _bikeService.GetBeachCruisersAsync();
        return Ok(bikes);
    }

    [HttpGet("mountain")]
    public async Task<ActionResult<List<MountainBikeDto>>> GetMountainBikes()
    {
        var bikes = await _bikeService.GetMountainBikesAsync();
        return Ok(bikes);
    }

    [HttpPost("rent")]
    public async Task<ActionResult<RentBikeResponse>> RentBike([FromBody] RentBikeRequest request)
    {
        var result = await _bikeService.RentBikeAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset")]
    public async Task<ActionResult<ResetResponse>> ResetFleet()
    {
        var result = await _bikeService.ResetFleetAsync();
        return Ok(result);
    }
}
