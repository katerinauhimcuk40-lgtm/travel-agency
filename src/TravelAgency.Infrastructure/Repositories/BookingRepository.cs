using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(TravelAgencyDbContext context) : base(context)
    {
    }

    public async Task<Booking?> GetWithDetailsAsync(int id) =>
        await WithDetails().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IReadOnlyList<Booking>> GetByUserWithDetailsAsync(int userId) =>
        await WithDetails()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.Id)
            .ToListAsync();

    public async Task<PagedResult<Booking>> SearchAsync(BookingSearchCriteria criteria)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(b => b.User)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Tour)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Hotel)
            .AsQueryable();

        if (criteria.Status.HasValue)
        {
            var status = criteria.Status.Value;
            query = query.Where(b => b.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Customer))
        {
            var text = criteria.Customer.Trim();
            query = query.Where(b => b.User!.Email.Contains(text) || b.User!.FullName.Contains(text));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return new PagedResult<Booking>(items, criteria.Page, criteria.PageSize, total);
    }

    public async Task<Booking?> GetForStaffAsync(int id) =>
        await WithDetails()
            .Include(b => b.User)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id);

    private IQueryable<Booking> WithDetails() =>
        _context.Bookings
            .AsNoTracking()
            .Include(b => b.Tourists)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Tour)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Hotel);
}
