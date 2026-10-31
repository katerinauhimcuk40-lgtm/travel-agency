using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Dtos;

public record CustomerBrief(int Id, string FullName, string Email, string? Phone)
{
    public static CustomerBrief From(User? user) => user is null
        ? new CustomerBrief(0, string.Empty, string.Empty, null)
        : new CustomerBrief(user.Id, user.FullName, user.Email, user.Phone);
}

/// <summary>Рядок списку заявок у кабінеті менеджера.</summary>
public record BookingSummaryResponse(
    int Id,
    CustomerBrief Customer,
    string TourTitle,
    string Country,
    string GroupName,
    DateTime DepartureDate,
    DateTime ArrivalDate,
    int PersonsCount,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAt)
{
    public static BookingSummaryResponse From(Booking booking) => new(
        booking.Id,
        CustomerBrief.From(booking.User),
        booking.TourGroup?.Tour?.Title ?? string.Empty,
        booking.TourGroup?.Tour?.Country ?? string.Empty,
        booking.TourGroup?.Name ?? string.Empty,
        booking.TourGroup?.DepartureDate ?? default,
        booking.TourGroup?.ArrivalDate ?? default,
        booking.PersonsCount,
        booking.TotalPrice,
        booking.Status,
        booking.CreatedAt);
}

/// <summary>Мандрівник для менеджера: паспорт показується лише у замаскованому вигляді.</summary>
public record CrmTouristResponse(
    int Id,
    string FullName,
    Gender Gender,
    int Age,
    string? ChildrenInfo,
    string PassportMasked,
    decimal TicketPrice);

public record PaymentResponse(
    int Id,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string Provider,
    DateTime CreatedAt,
    DateTime? PaidAt)
{
    public static PaymentResponse From(Payment payment) => new(
        payment.Id,
        payment.Amount,
        payment.Currency,
        payment.Status,
        payment.Provider,
        payment.CreatedAt,
        payment.PaidAt);
}

public record CrmBookingDetailsResponse(
    int Id,
    CustomerBrief Customer,
    int TourId,
    string TourTitle,
    string Country,
    string Cities,
    string GroupName,
    DateTime DepartureDate,
    DateTime ArrivalDate,
    string HotelName,
    string HotelType,
    int PersonsCount,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt,
    List<CrmTouristResponse> Tourists,
    List<PaymentResponse> Payments)
{
    /// <param name="maskPassport">Перетворює зашифрований паспорт на замаскований рядок.</param>
    public static CrmBookingDetailsResponse From(Booking booking, Func<string, string> maskPassport)
    {
        var group = booking.TourGroup;
        var tour = group?.Tour;
        var hotel = group?.Hotel;

        return new CrmBookingDetailsResponse(
            booking.Id,
            CustomerBrief.From(booking.User),
            tour?.Id ?? 0,
            tour?.Title ?? string.Empty,
            tour?.Country ?? string.Empty,
            tour?.Cities ?? string.Empty,
            group?.Name ?? string.Empty,
            group?.DepartureDate ?? default,
            group?.ArrivalDate ?? default,
            hotel?.Name ?? string.Empty,
            hotel?.HotelType ?? string.Empty,
            booking.PersonsCount,
            booking.TotalPrice,
            booking.Status,
            booking.CreatedAt,
            booking.ConfirmedAt,
            booking.CancelledAt,
            booking.Tourists
                .Select(t => new CrmTouristResponse(
                    t.Id,
                    t.FullName,
                    t.Gender,
                    t.Age,
                    t.ChildrenInfo,
                    maskPassport(t.PassportEncrypted),
                    t.TicketPrice))
                .ToList(),
            booking.Payments
                .OrderBy(p => p.CreatedAt)
                .Select(PaymentResponse.From)
                .ToList());
    }
}

public record CustomerResponse(
    int Id,
    string Email,
    string FullName,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt,
    int BookingsCount)
{
    public static CustomerResponse From(CustomerSummary customer) => new(
        customer.Id,
        customer.Email,
        customer.FullName,
        customer.Phone,
        customer.IsActive,
        customer.CreatedAt,
        customer.BookingsCount);
}
