using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class TourCatalogServiceTests
{
    /// <summary>
    /// Дані: Paris (France, BB, 800) — групи через 30 і 60 днів; Antalya (Turkey, AI, 600) — групи через 20 днів і вчора;
    /// неактивний тур Italy — група через 10 днів.
    /// </summary>
    private static async Task<(TravelAgencyDbContext Context, TourCatalogService Service)> ArrangeAsync()
    {
        var context = TestDb.CreateContext();
        var now = DateTime.UtcNow;

        var lumiere = new Hotel { Id = 1, Name = "Hotel Lumiere", Country = "France", City = "Paris", Address = "Rue 1", Capacity = 10, HotelType = "4*" };
        var sunrise = new Hotel { Id = 2, Name = "Sunrise Resort", Country = "Turkey", City = "Antalya", Address = "Beach 1", Capacity = 10, HotelType = "5*" };

        var paris = new Tour { Id = 1, Title = "Paris", Country = "France", Cities = "Paris", MealPlan = MealPlan.BB, AccommodationType = "4*", BasePrice = 800 };
        var antalya = new Tour { Id = 2, Title = "Antalya", Country = "Turkey", Cities = "Antalya", MealPlan = MealPlan.AI, AccommodationType = "5*", BasePrice = 600 };
        var hidden = new Tour { Id = 3, Title = "Hidden", Country = "Italy", Cities = "Rome", MealPlan = MealPlan.BB, AccommodationType = "3*", BasePrice = 500, IsActive = false };

        context.Hotels.AddRange(lumiere, sunrise);
        context.Tours.AddRange(paris, antalya, hidden);
        context.TourGroups.AddRange(
            Group(1, "PAR-01", paris, lumiere, now.AddDays(30)),
            Group(2, "PAR-02", paris, lumiere, now.AddDays(60)),
            Group(3, "ANT-01", antalya, sunrise, now.AddDays(20)),
            Group(4, "ANT-OLD", antalya, sunrise, now.AddDays(-1)),
            Group(5, "HID-01", hidden, lumiere, now.AddDays(10)));
        await context.SaveChangesAsync();

        return (context, new TourCatalogService(new UnitOfWork(context)));
    }

    private static TourGroup Group(int id, string name, Tour tour, Hotel hotel, DateTime departure) => new()
    {
        Id = id,
        Name = name,
        TourId = tour.Id,
        HotelId = hotel.Id,
        DepartureDate = departure,
        ArrivalDate = departure.AddDays(7),
        SeatsTotal = 10,
        SeatsAvailable = 10
    };

    private static string[] Names(PagedResult<TourGroup> result) => result.Items.Select(g => g.Name).ToArray();

    [Fact]
    public async Task Search_NoFilters_ReturnsUpcomingActiveGroupsSortedByDeparture()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria());

        Assert.Equal(new[] { "ANT-01", "PAR-01", "PAR-02" }, Names(result));
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, g => Assert.NotNull(g.Tour));
        Assert.All(result.Items, g => Assert.NotNull(g.Hotel));
    }

    [Fact]
    public async Task Search_ByCountry_ReturnsOnlyThatCountry()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { Country = "France" });

        Assert.Equal(new[] { "PAR-01", "PAR-02" }, Names(result));
    }

    [Fact]
    public async Task Search_ByMaxPrice_ReturnsCheaperTours()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { MaxPrice = 700 });

        Assert.Equal(new[] { "ANT-01" }, Names(result));
    }

    [Fact]
    public async Task Search_ByMinPrice_ReturnsMoreExpensiveTours()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { MinPrice = 700 });

        Assert.Equal(new[] { "PAR-01", "PAR-02" }, Names(result));
    }

    [Fact]
    public async Task Search_ByMealPlan_ReturnsMatchingTours()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { MealPlan = MealPlan.AI });

        Assert.Equal(new[] { "ANT-01" }, Names(result));
    }

    [Fact]
    public async Task Search_ByHotelName_ReturnsMatchingGroups()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { Hotel = "Sunrise" });

        Assert.Equal(new[] { "ANT-01" }, Names(result));
    }

    [Fact]
    public async Task Search_ByDepartureRange_ReturnsGroupsInsideRange()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;
        var now = DateTime.UtcNow;

        var result = await service.SearchAsync(new TourSearchCriteria
        {
            DepartureFrom = now.AddDays(25),
            DepartureTo = now.AddDays(45)
        });

        Assert.Equal(new[] { "PAR-01" }, Names(result));
    }

    [Fact]
    public async Task Search_Paging_ReturnsRequestedPage()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { Page = 2, PageSize = 2 });

        Assert.Equal(new[] { "PAR-02" }, Names(result));
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task Search_TooLargePageSize_IsClampedTo100()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.SearchAsync(new TourSearchCriteria { PageSize = 1000, Page = -5 });

        Assert.Equal(100, result.PageSize);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task Search_DepartureFromAfterTo_Throws()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;
        var now = DateTime.UtcNow;

        await Assert.ThrowsAsync<ArgumentException>(() => service.SearchAsync(new TourSearchCriteria
        {
            DepartureFrom = now.AddDays(10),
            DepartureTo = now.AddDays(5)
        }));
    }

    [Fact]
    public async Task Search_MinPriceGreaterThanMaxPrice_Throws()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        await Assert.ThrowsAsync<ArgumentException>(() => service.SearchAsync(new TourSearchCriteria
        {
            MinPrice = 900,
            MaxPrice = 100
        }));
    }

    [Fact]
    public async Task GetTours_ReturnsOnlyActiveTours()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var result = await service.GetToursAsync(1, 10);

        Assert.Equal(new[] { "Antalya", "Paris" }, result.Items.Select(t => t.Title).ToArray());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetTour_ReturnsTourWithUpcomingGroupsOnly()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        var tour = await service.GetTourAsync(2);

        var group = Assert.Single(tour.TourGroups);
        Assert.Equal("ANT-01", group.Name);
        Assert.NotNull(group.Hotel);
    }

    [Fact]
    public async Task GetTour_InactiveTour_Throws()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTourAsync(3));
    }

    [Fact]
    public async Task GetTour_NotFound_Throws()
    {
        var (context, service) = await ArrangeAsync();
        using var _ = context;

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTourAsync(999));
    }
}
