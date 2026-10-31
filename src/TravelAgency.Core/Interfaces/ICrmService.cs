using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

/// <summary>Читання даних для кабінету менеджера (UC-11, UC-13, UC-14). Зміни статусів виконує IBookingService.</summary>
public interface ICrmService
{
    /// <summary>Заявки всіх клієнтів (нові першими) з клієнтом, туром, групою та готелем.</summary>
    Task<PagedResult<Booking>> SearchBookingsAsync(BookingSearchCriteria criteria);

    /// <summary>Заявка з клієнтом, мандрівниками та оплатами. Немає заявки — KeyNotFoundException.</summary>
    Task<Booking> GetBookingAsync(int bookingId);

    /// <summary>Клієнти з пошуком за іменем або поштою.</summary>
    Task<PagedResult<CustomerSummary>> GetCustomersAsync(string? search, int page, int pageSize);
}
