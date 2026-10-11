using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class TourGroupRepository : Repository<TourGroup>, ITourGroupRepository
{
    public TourGroupRepository(TravelAgencyDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<TourGroup>> SearchAsync(TourSearchCriteria criteria, DateTime now)
    {
        var query = _context.TourGroups
            .AsNoTracking()
            .Include(g => g.Tour)
            .Include(g => g.Hotel)
            .Where(g => g.Tour!.IsActive && g.DepartureDate > now);

        if (!string.IsNullOrWhiteSpace(criteria.Country))
        {
            var country = criteria.Country.Trim();
            query = query.Where(g => g.Tour!.Country == country);
        }

        if (criteria.DepartureFrom.HasValue)
        {
            var from = criteria.DepartureFrom.Value;
            query = query.Where(g => g.DepartureDate >= from);
        }

        if (criteria.DepartureTo.HasValue)
        {
            var to = criteria.DepartureTo.Value;
            query = query.Where(g => g.DepartureDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Hotel))
        {
            var hotel = criteria.Hotel.Trim();
            query = query.Where(g => g.Hotel!.Name.Contains(hotel));
        }

        if (criteria.MinPrice.HasValue)
        {
            var min = criteria.MinPrice.Value;
            query = query.Where(g => g.Tour!.BasePrice >= min);
        }

        if (criteria.MaxPrice.HasValue)
        {
            var max = criteria.MaxPrice.Value;
            query = query.Where(g => g.Tour!.BasePrice <= max);
        }

        if (criteria.MealPlan.HasValue)
        {
            var meal = criteria.MealPlan.Value;
            query = query.Where(g => g.Tour!.MealPlan == meal);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(g => g.DepartureDate)
            .ThenBy(g => g.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return new PagedResult<TourGroup>(items, criteria.Page, criteria.PageSize, total);
    }
}
