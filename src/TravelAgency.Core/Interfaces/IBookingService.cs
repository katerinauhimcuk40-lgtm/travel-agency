using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, int tourGroupId, IReadOnlyList<TouristInfo> tourists);
    Task CancelBookingAsync(int bookingId);
    Task ConfirmBookingAsync(int bookingId);
}
