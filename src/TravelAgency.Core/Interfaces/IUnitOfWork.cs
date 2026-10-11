using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    ITourRepository Tours { get; }
    IRepository<Hotel> Hotels { get; }
    ITourGroupRepository TourGroups { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Tourist> Tourists { get; }
    IRepository<Payment> Payments { get; }
    Task<int> SaveChangesAsync();
}
