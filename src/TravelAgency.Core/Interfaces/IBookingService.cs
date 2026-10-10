using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, int tourGroupId, List<Tourist> tourists);
    Task CancelBookingAsync(int bookingId);
    Task ConfirmBookingAsync(int bookingId);
}