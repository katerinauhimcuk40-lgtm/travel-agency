using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class DbSeederTests
{
    [Fact]
    public async Task Seed_FillsDatabaseWithConsistentData()
    {
        using var context = TestDb.CreateContext();

        await DbSeeder.SeedAsync(context);

        Assert.Equal(3, await context.Users.CountAsync());
        Assert.Equal(5, await context.Hotels.CountAsync());
        Assert.Equal(4, await context.Tours.CountAsync());
        Assert.Equal(7, await context.TourGroups.CountAsync());

        var groups = await context.TourGroups.ToListAsync();
        Assert.All(groups, g =>
        {
            Assert.True(g.TourId > 0);
            Assert.True(g.HotelId > 0);
            Assert.True(g.DepartureDate > DateTime.UtcNow);
            Assert.True(g.ArrivalDate >= g.DepartureDate);
            Assert.Equal(g.SeatsTotal, g.SeatsAvailable);
        });
    }

    [Fact]
    public async Task Seed_CalledTwice_DoesNotDuplicateData()
    {
        using var context = TestDb.CreateContext();

        await DbSeeder.SeedAsync(context);
        await DbSeeder.SeedAsync(context);

        Assert.Equal(3, await context.Users.CountAsync());
        Assert.Equal(4, await context.Tours.CountAsync());
        Assert.Equal(7, await context.TourGroups.CountAsync());
    }

    [Fact]
    public async Task SeededData_CanBeFoundByCatalogSearch()
    {
        using var context = TestDb.CreateContext();
        await DbSeeder.SeedAsync(context);
        var service = new TourCatalogService(new UnitOfWork(context));

        var result = await service.SearchAsync(new TourSearchCriteria { Country = "Turkey" });

        Assert.Equal(2, result.TotalCount);
    }
}
