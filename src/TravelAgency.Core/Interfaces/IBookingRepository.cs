using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    /// <summary>Бронювання разом з мандрівниками, групою, туром і готелем.</summary>
    Task<Booking?> GetWithDetailsAsync(int id);

    /// <summary>Усі бронювання користувача (нові першими) з деталями туру.</summary>
    Task<IReadOnlyList<Booking>> GetByUserWithDetailsAsync(int userId);

    /// <summary>Заявки всіх клієнтів за статусом і текстом (ім'я або пошта клієнта), нові першими.</summary>
    Task<PagedResult<Booking>> SearchAsync(BookingSearchCriteria criteria);

    /// <summary>Бронювання з клієнтом, мандрівниками, групою, туром, готелем та оплатами — для менеджера.</summary>
    Task<Booking?> GetForStaffAsync(int id);
}
