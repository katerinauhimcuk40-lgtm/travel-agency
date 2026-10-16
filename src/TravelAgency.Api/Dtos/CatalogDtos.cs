using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Dtos;

public record HotelResponse(int Id, string Name, string Country, string City, string HotelType)
{
    public static HotelResponse From(Hotel hotel) =>
        new(hotel.Id, hotel.Name, hotel.Country, hotel.City, hotel.HotelType);
}

public record TourResponse(
    int Id,
    string Title,
    string? Description,
    string Country,
    string Cities,
    TransportType TransportType,
    MealPlan MealPlan,
    string AccommodationType,
    decimal BasePrice)
{
    public static TourResponse From(Tour tour) => new(
        tour.Id,
        tour.Title,
        tour.Description,
        tour.Country,
        tour.Cities,
        tour.TransportType,
        tour.MealPlan,
        tour.AccommodationType,
        tour.BasePrice);
}

public record TourGroupResponse(
    int Id,
    string Name,
    DateTime DepartureDate,
    DateTime ArrivalDate,
    int SeatsTotal,
    int SeatsAvailable,
    HotelResponse? Hotel)
{
    public static TourGroupResponse From(TourGroup group) => new(
        group.Id,
        group.Name,
        group.DepartureDate,
        group.ArrivalDate,
        group.SeatsTotal,
        group.SeatsAvailable,
        group.Hotel is null ? null : HotelResponse.From(group.Hotel));
}

public record TourDetailsResponse(TourResponse Tour, List<TourGroupResponse> Groups)
{
    public static TourDetailsResponse From(Tour tour) => new(
        TourResponse.From(tour),
        tour.TourGroups.Select(TourGroupResponse.From).ToList());
}

public record TourSearchItem(TourResponse Tour, TourGroupResponse Group)
{
    public static TourSearchItem From(TourGroup group) =>
        new(TourResponse.From(group.Tour!), TourGroupResponse.From(group));
}

public record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages)
{
    public static PagedResponse<T> From<TSource>(PagedResult<TSource> result, Func<TSource, T> map) => new(
        result.Items.Select(map).ToList(),
        result.Page,
        result.PageSize,
        result.TotalCount,
        result.TotalPages);
}
public record AvailabilityResponse(int GroupId, int SeatsTotal, int SeatsAvailable, bool IsAvailable);