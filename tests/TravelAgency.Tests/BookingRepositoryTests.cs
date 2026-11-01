using Xunit;

using TravelAgency.Core.Entities;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace TravelAgency.Tests;

public class BookingRepositoryTests
{
    [Fact]
    public async Task GetWithDetailsAsync_ShouldReturnBookingWithRelations()
    {
        using var context = TestDb.CreateContext();
        var repository = new BookingRepository(context);
        var group = await TestDb.SeedGroupAsync(context);

        // Створюємо тестове бронювання (без прямого присвоєння read-only властивостей)
        var booking = new Booking
        {
            TourGroupId = group.Id,
            UserId = 1,
            TotalPrice = 1000
        };

        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Викликаємо метод репозиторію
        var result = await repository.GetWithDetailsAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserWithDetailsAsync_ShouldReturnUserBookings()
    {
        using var context = TestDb.CreateContext();
        var repository = new BookingRepository(context);
        var group = await TestDb.SeedGroupAsync(context);

        var booking = new Booking
        {
            TourGroupId = group.Id,
            UserId = 5,
            TotalPrice = 1500
        };

        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Тестуємо вибірку за користувачем
        var results = await repository.GetByUserWithDetailsAsync(5);

        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(5, results[0].UserId);
    }
}