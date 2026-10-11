using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface ITourCatalogService
{
    /// <summary>UC-03: переглянути тури.</summary>
    Task<PagedResult<Tour>> GetToursAsync(int page, int pageSize);

    /// <summary>UC-04: інформація про тур з найближчими групами.</summary>
    Task<Tour> GetTourAsync(int id);

    /// <summary>UC-05: пошук груп за критеріями.</summary>
    Task<PagedResult<TourGroup>> SearchAsync(TourSearchCriteria criteria);
}
