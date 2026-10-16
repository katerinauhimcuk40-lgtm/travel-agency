using Microsoft.AspNetCore.Mvc;

using TravelAgency.Api.Dtos;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly ITourCatalogService _catalog;

    public ToursController(ITourCatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TourResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _catalog.GetToursAsync(page, pageSize);
        return Ok(PagedResponse<TourResponse>.From(result, TourResponse.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TourDetailsResponse>> GetById(int id)
    {
        var tour = await _catalog.GetTourAsync(id);
        return Ok(TourDetailsResponse.From(tour));
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<TourSearchItem>>> Search([FromQuery] TourSearchCriteria criteria)
    {
        var result = await _catalog.SearchAsync(criteria);
        return Ok(PagedResponse<TourSearchItem>.From(result, TourSearchItem.From));
    }
    [HttpGet("groups/{groupId:int}/availability")]
    [ProducesResponseType<AvailabilityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AvailabilityResponse>> GetAvailability(int groupId)
    {
        var g = await _catalog.GetGroupAsync(groupId);
        return Ok(new AvailabilityResponse(g.Id, g.SeatsTotal, g.SeatsAvailable, g.SeatsAvailable > 0));
    }
}
