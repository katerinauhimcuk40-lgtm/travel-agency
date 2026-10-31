using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    ITourRepository Tours { get; }
    IRepository<Hotel> Hotels { get; }
    ITourGroupRepository TourGroups { get; }
    IBookingRepository Bookings { get; }
    IRepository<Tourist> Tourists { get; }
    IRepository<Payment> Payments { get; }
    Task<int> SaveChangesAsync();
}
