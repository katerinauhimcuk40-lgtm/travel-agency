using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TravelAgencyDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<Tour> Tours { get; }
    public IRepository<Hotel> Hotels { get; }
    public IRepository<TourGroup> TourGroups { get; }
    public IRepository<Booking> Bookings { get; }
    public IRepository<Tourist> Tourists { get; }
    public IRepository<Payment> Payments { get; }

    public UnitOfWork(TravelAgencyDbContext context)
    {
        _context = context;
        Users = new Repository<User>(_context);
        Tours = new Repository<Tour>(_context);
        Hotels = new Repository<Hotel>(_context);
        TourGroups = new Repository<TourGroup>(_context);
        Bookings = new Repository<Booking>(_context);
        Tourists = new Repository<Tourist>(_context);
        Payments = new Repository<Payment>(_context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}