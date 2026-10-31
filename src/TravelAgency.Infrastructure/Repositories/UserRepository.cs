using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(TravelAgencyDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<CustomerSummary>> SearchCustomersAsync(string? search, int page, int pageSize)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(u => u.Role == Role.Client);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim();
            query = query.Where(u => u.Email.Contains(text) || u.FullName.Contains(text));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FullName)
            .ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new CustomerSummary(
                u.Id,
                u.Email,
                u.FullName,
                u.Phone,
                u.IsActive,
                u.CreatedAt,
                u.Bookings.Count))
            .ToListAsync();

        return new PagedResult<CustomerSummary>(items, page, pageSize, total);
    }
}
