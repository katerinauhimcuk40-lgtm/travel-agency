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
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

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
