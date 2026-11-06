using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

/// <summary>
/// Стан бронювання (патерн State). Кожен метод повертає статус,
/// у який бронювання переходить, або кидає виняток, якщо перехід заборонений.
/// </summary>
public interface IBookingState
{
    BookingStatus Confirm();
    BookingStatus Pay();
    BookingStatus Cancel();
    BookingStatus Complete();
}