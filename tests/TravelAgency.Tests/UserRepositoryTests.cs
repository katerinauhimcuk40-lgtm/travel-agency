using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;

using Xunit;

namespace TravelAgency.Tests;

public class UserRepositoryTests
{
    private static TravelAgencyDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TravelAgencyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TravelAgencyDbContext(options);
    }

    private static User NewUser(string email, string fullName, Role role = Role.Client) => new()
    {
        Email = email,
        FullName = fullName,
        Role = role,
        IsActive = true
    };

    private static async Task<UserRepository> SeedAsync(TravelAgencyDbContext context)
    {
        context.Users.AddRange(
            NewUser("bohdan@test.com", "Бондар Богдан"),
            NewUser("anna@test.com", "Анна Іваненко"),
            NewUser("olena@test.com", "Олена Петренко"),
            NewUser("manager@test.com", "Менеджер Тест", Role.Manager),
            NewUser("admin@test.com", "Адмін Тест", Role.Admin));
        await context.SaveChangesAsync();
        return new UserRepository(context);
    }

    [Fact]
    public async Task SearchCustomersAsync_NoSearch_ReturnsOnlyClientsOrderedByName()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync(null, 1, 10);

        Assert.Equal(3, result.Items.Count);
        Assert.DoesNotContain(result.Items, c => c.Email == "manager@test.com");
        Assert.DoesNotContain(result.Items, c => c.Email == "admin@test.com");
        var names = result.Items.Select(c => c.FullName).ToList();
        Assert.Equal(names.OrderBy(n => n).ToList(), names);
    }

    [Fact]
    public async Task SearchCustomersAsync_SearchByEmail_ReturnsMatch()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync("anna@", 1, 10);

        var customer = Assert.Single(result.Items);
        Assert.Equal("anna@test.com", customer.Email);
    }

    [Fact]
    public async Task SearchCustomersAsync_SearchByFullName_ReturnsMatch()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync("Петренко", 1, 10);

        var customer = Assert.Single(result.Items);
        Assert.Equal("olena@test.com", customer.Email);
    }

    [Fact]
    public async Task SearchCustomersAsync_SearchIsTrimmed()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync("  anna@  ", 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task SearchCustomersAsync_WhitespaceSearch_ReturnsAllClients()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync("   ", 1, 10);

        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task SearchCustomersAsync_NoMatches_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var result = await repository.SearchCustomersAsync("такого-немає", 1, 10);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task SearchCustomersAsync_Paging_ReturnsRequestedPage()
    {
        using var context = CreateContext();
        var repository = await SeedAsync(context);

        var page1 = await repository.SearchCustomersAsync(null, 1, 2);
        var page2 = await repository.SearchCustomersAsync(null, 2, 2);

        Assert.Equal(2, page1.Items.Count);
        Assert.Single(page2.Items);
        Assert.DoesNotContain(page2.Items, c => page1.Items.Any(p => p.Id == c.Id));
    }
}