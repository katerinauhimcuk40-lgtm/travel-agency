using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface ITourCatalogService
{
    Task<PagedResult<Tour>> GetToursAsync(int page, int pageSize);

    Task<Tour> GetTourAsync(int id);

    Task<PagedResult<TourGroup>> SearchAsync(TourSearchCriteria criteria);
    Task<TourGroup> GetGroupAsync(int groupId);
}
