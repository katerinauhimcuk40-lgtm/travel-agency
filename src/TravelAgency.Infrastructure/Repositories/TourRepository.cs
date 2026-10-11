using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class TourRepository : Repository<Tour>, ITourRepository
{
    public TourRepository(TravelAgencyDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Tour>> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Tours.AsNoTracking().Where(t => t.IsActive);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(t => t.Title)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Tour>(items, page, pageSize, total);
    }

    public async Task<Tour?> GetWithUpcomingGroupsAsync(int id, DateTime now)
    {
        return await _context.Tours
            .AsNoTracking()
            .Include(t => t.TourGroups.Where(g => g.DepartureDate > now).OrderBy(g => g.DepartureDate))
                .ThenInclude(g => g.Hotel)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
