using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, int tourGroupId, IReadOnlyList<TouristInfo> tourists);
    Task CancelBookingAsync(int bookingId);
    Task ConfirmBookingAsync(int bookingId);

    /// <summary>Бронювання з деталями туру (UC-08). Немає бронювання — KeyNotFoundException.</summary>
    Task<Booking> GetBookingAsync(int bookingId);

    /// <summary>Усі бронювання користувача (UC-08).</summary>
    Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId);
}
