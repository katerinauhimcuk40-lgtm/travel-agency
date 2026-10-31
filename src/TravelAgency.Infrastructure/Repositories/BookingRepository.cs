using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
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

    private IQueryable<Booking> WithDetails() =>
        _context.Bookings
            .AsNoTracking()
            .Include(b => b.Tourists)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Tour)
            .Include(b => b.TourGroup).ThenInclude(g => g!.Hotel);
}
