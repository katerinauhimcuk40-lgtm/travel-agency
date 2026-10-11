using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Services;

public class TourCatalogService : ITourCatalogService
{
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public TourCatalogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<Tour>> GetToursAsync(int page, int pageSize)
    {
        var (safePage, safePageSize) = NormalizePaging(page, pageSize);
        return await _unitOfWork.Tours.GetPagedAsync(safePage, safePageSize);
    }

    public async Task<Tour> GetTourAsync(int id)
    {
        var tour = await _unitOfWork.Tours.GetWithUpcomingGroupsAsync(id, DateTime.UtcNow);

        if (tour is null || !tour.IsActive)
            throw new KeyNotFoundException("Tour not found");

        return tour;
    }

    public async Task<PagedResult<TourGroup>> SearchAsync(TourSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (criteria.DepartureFrom.HasValue && criteria.DepartureTo.HasValue
            && criteria.DepartureFrom > criteria.DepartureTo)
            throw new ArgumentException("DepartureFrom must not be later than DepartureTo.");

        if (criteria.MinPrice.HasValue && criteria.MaxPrice.HasValue
            && criteria.MinPrice > criteria.MaxPrice)
            throw new ArgumentException("MinPrice must not be greater than MaxPrice.");

        (criteria.Page, criteria.PageSize) = NormalizePaging(criteria.Page, criteria.PageSize);

        return await _unitOfWork.TourGroups.SearchAsync(criteria, DateTime.UtcNow);
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize) =>
        (Math.Max(1, page), Math.Clamp(pageSize, 1, MaxPageSize));
}
