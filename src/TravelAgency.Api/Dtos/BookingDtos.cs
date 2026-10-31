using System.ComponentModel.DataAnnotations;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;

namespace TravelAgency.Api.Dtos;

public class TouristRequest
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Passport { get; set; } = string.Empty;

    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [Range(1, 120)]
    public int Age { get; set; }

    [StringLength(50)]
    public string? ChildrenInfo { get; set; }
}

public class CreateBookingRequest
{
    /// <summary>Клієнт, на якого оформлюється бронювання. Лише для менеджера/адміністратора (UC-12);
    /// для клієнта ігнорується — використовується його власний Id із токена.</summary>
    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int TourGroupId { get; set; }

    [Required, MinLength(1)]
    public List<TouristRequest> Tourists { get; set; } = [];
}

public class PayRequest
{
    [Range(0.01, 1000000000)]
    public decimal Amount { get; set; }
}

/// <summary>Відповідь не містить паспортних даних.</summary>
public record TouristResponse(int Id, string FullName, Gender Gender, int Age, string? ChildrenInfo, decimal TicketPrice);

/// <summary>Бронювання з даними туру, групи та готелю — для кабінету клієнта (UC-08).</summary>
public record BookingDetailsResponse(
    int Id,
    int UserId,
    int TourId,
    string TourTitle,
    string Country,
    string Cities,
    int TourGroupId,
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
    List<TouristResponse> Tourists)
{
    public static BookingDetailsResponse From(Booking booking)
    {
        var group = booking.TourGroup;
        var tour = group?.Tour;
        var hotel = group?.Hotel;

        return new BookingDetailsResponse(
            booking.Id,
            booking.UserId,
            tour?.Id ?? 0,
            tour?.Title ?? string.Empty,
            tour?.Country ?? string.Empty,
            tour?.Cities ?? string.Empty,
            booking.TourGroupId,
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
            booking.Tourists
                .Select(t => new TouristResponse(t.Id, t.FullName, t.Gender, t.Age, t.ChildrenInfo, t.TicketPrice))
                .ToList());
    }
}

public record BookingResponse(
    int Id,
    int UserId,
    int TourGroupId,
    int PersonsCount,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAt,
    List<TouristResponse> Tourists)
{
    public static BookingResponse From(Booking booking) => new(
        booking.Id,
        booking.UserId,
        booking.TourGroupId,
        booking.PersonsCount,
        booking.TotalPrice,
        booking.Status,
        booking.CreatedAt,
        booking.Tourists
            .Select(t => new TouristResponse(t.Id, t.FullName, t.Gender, t.Age, t.ChildrenInfo, t.TicketPrice))
            .ToList());
}
