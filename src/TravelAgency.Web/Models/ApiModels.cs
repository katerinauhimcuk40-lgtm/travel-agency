using TravelAgency.Core.Enums;

namespace TravelAgency.Web.Models;

// Моделі відповідей REST API (дзеркало DTO з TravelAgency.Api/Dtos).

public record HotelResponse(int Id, string Name, string Country, string City, string HotelType);

public record TourResponse(
    int Id,
    string Title,
    string? Description,
    string Country,
    string Cities,
    TransportType TransportType,
    MealPlan MealPlan,
    string AccommodationType,
    decimal BasePrice);

public record TourGroupResponse(
    int Id,
    string Name,
    DateTime DepartureDate,
    DateTime ArrivalDate,
    int SeatsTotal,
    int SeatsAvailable,
    HotelResponse? Hotel);

public record TourDetailsResponse(TourResponse Tour, List<TourGroupResponse> Groups);

public record TourSearchItem(TourResponse Tour, TourGroupResponse Group);

public record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public record AuthResponse(string Token, DateTime ExpiresAt, int UserId, string Email, string FullName, Role Role);

public record TouristResponse(int Id, string FullName, Gender Gender, int Age, string? ChildrenInfo, decimal TicketPrice);

public record BookingResponse(
    int Id,
    int UserId,
    int TourGroupId,
    int PersonsCount,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAt,
    List<TouristResponse> Tourists);

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
    List<TouristResponse> Tourists);

// ----- Кабінет менеджера (api/crm) -----

public record CustomerBrief(int Id, string FullName, string Email, string? Phone);

public record BookingSummary(
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
    DateTime CreatedAt);

public record CrmTourist(
    int Id,
    string FullName,
    Gender Gender,
    int Age,
    string? ChildrenInfo,
    string PassportMasked,
    decimal TicketPrice);

public record PaymentInfo(
    int Id,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string Provider,
    DateTime CreatedAt,
    DateTime? PaidAt);

public record CrmBookingDetails(
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
    List<CrmTourist> Tourists,
    List<PaymentInfo> Payments);

public record CustomerInfo(
    int Id,
    string Email,
    string FullName,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt,
    int BookingsCount);

/// <summary>Параметри пошуку турів (GET api/tours/search).</summary>
public record TourSearchQuery(
    string? Country,
    DateTime? From,
    DateTime? To,
    string? Hotel,
    decimal? MinPrice,
    decimal? MaxPrice,
    MealPlan? Meal,
    int Page,
    int PageSize);
