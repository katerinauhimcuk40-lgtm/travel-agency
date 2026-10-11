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

    /// <summary>UC-03: список активних турів.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<TourResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _catalog.GetToursAsync(page, pageSize);
        return Ok(PagedResponse<TourResponse>.From(result, TourResponse.From));
    }

    /// <summary>UC-04: інформація про тур і його майбутні групи.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TourDetailsResponse>> GetById(int id)
    {
        var tour = await _catalog.GetTourAsync(id);
        return Ok(TourDetailsResponse.From(tour));
    }

    /// <summary>UC-05: пошук груп за країною, датами вильоту, готелем, ціною та харчуванням.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<TourSearchItem>>> Search([FromQuery] TourSearchCriteria criteria)
    {
        var result = await _catalog.SearchAsync(criteria);
        return Ok(PagedResponse<TourSearchItem>.From(result, TourSearchItem.From));
    }
}
