
using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationService;

    public LocationsController(ILocationsService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAllLocations()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetLocationById(int id)
    {
        var location = await _locationService.GetByIdAsync(id);

        if (location is null)
            return NotFound();

        return Ok(location);
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> CreateLocation(
        CreateLocationDto dto)
    {
        try
        {
            var location = await _locationService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetLocationById),
                new { id = location.Id },
                location);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LocationDto>> UpdateLocation(
        int id, UpdatedLocationDto dto)
    {
        try
        {
            var location = await _locationService.UpdateAsync(id, dto);

            if (location is null)
                return NotFound();

            return Ok(location);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteLocation(int id)
    {
        var result = await _locationService.DeleteAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
