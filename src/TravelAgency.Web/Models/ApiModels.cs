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
