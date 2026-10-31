using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    /// <summary>Бронювання разом з мандрівниками, групою, туром і готелем.</summary>
    Task<Booking?> GetWithDetailsAsync(int id);

    /// <summary>Усі бронювання користувача (нові першими) з деталями туру.</summary>
    Task<IReadOnlyList<Booking>> GetByUserWithDetailsAsync(int userId);
}
