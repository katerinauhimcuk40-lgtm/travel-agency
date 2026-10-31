using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TravelAgencyDbContext _context;

    public IUserRepository Users { get; }
    public ITourRepository Tours { get; }
    public IRepository<Hotel> Hotels { get; }
    public ITourGroupRepository TourGroups { get; }
    public IBookingRepository Bookings { get; }
    public IRepository<Tourist> Tourists { get; }
    public IRepository<Payment> Payments { get; }

    public UnitOfWork(TravelAgencyDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Tours = new TourRepository(_context);
        Hotels = new Repository<Hotel>(_context);
        TourGroups = new TourGroupRepository(_context);
        Bookings = new BookingRepository(_context);
        Tourists = new Repository<Tourist>(_context);
        Payments = new Repository<Payment>(_context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
