using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Tour> Tours { get; }
    IRepository<Hotel> Hotels { get; }
    IRepository<TourGroup> TourGroups { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Tourist> Tourists { get; }
    IRepository<Payment> Payments { get; }
    Task<int> SaveChangesAsync();
}